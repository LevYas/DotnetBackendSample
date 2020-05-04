using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Auth;
using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Core.Food;
using SugarCounter.Core.Shared;
using SugarCounter.Core.Users;
using System;
using System.Threading.Tasks;

namespace SugarCounter.Api.Controllers.Food
{
    public class FoodController : BaseApiController
    {
        private readonly IFoodRepository _repository;
        private readonly RequestContext _requestContext;
        private readonly INutritionDataProvider _nutritionDataProvider;

        public FoodController(IFoodRepository repository, INutritionDataProvider nutritionDataProvider,
            RequestContext requestContext)
        {
            _repository = repository;
            _requestContext = requestContext;
            _nutritionDataProvider = nutritionDataProvider;
        }

        [HttpPost]
        public Task<ActionResult<FoodItemDto>> AddItem(FoodItemInputDto input) =>
            AddItemToUser(_requestContext.CurrentUser.Id, input);

        [HttpPost("forUser/{userId}")]
        [AuthorizeFor(UserRole.Admin)]
        public async Task<ActionResult<FoodItemDto>> AddItemToUser(int userId, FoodItemInputDto inputDto)
        {
            int sugar = await getSugarMass(inputDto);
            DateTime whenAdded = inputDto.WhenAdded ?? DateTime.Now;

            var newFood = new NewFoodInput(userId, inputDto.Description.Trim(), sugar, whenAdded);
            FoodItem? foodItem = await _repository.Create(newFood);

            if (foodItem != null)
                return new FoodItemDto(foodItem);
            else
                return Problem();
        }

        [HttpGet("{itemId}")]
        public async Task<ActionResult<FoodItemDto>> GetItem(int itemId)
        {
            FoodItem? foodItem = await _repository.GetById(itemId);

            if (checkForAccessError(itemId, foodItem) is ActionResult accessError)
                return accessError;

            return new FoodItemDto(foodItem!);
        }

        [HttpGet("myItems")]
        public Task<ActionResult<PaginatedListDto<FoodItemDto>>> GetItemsOfCurrentUser(
            [FromQuery] FoodRequestDto? requestParams = null)
        {
            return GetItemsOfUser(_requestContext.CurrentUser.Id, requestParams);
        }

        [HttpGet("ofUser/{userId}")]
        [AuthorizeFor(UserRole.Admin)]
        public Task<ActionResult<PaginatedListDto<FoodItemDto>>> GetItemsOfUser(int userId,
            [FromQuery] FoodRequestDto? requestParams = null)
            => getItems(requestParams, userId);

        [HttpGet]
        [AuthorizeFor(UserRole.Admin)]
        public Task<ActionResult<PaginatedListDto<FoodItemDto>>> GetAllItems(
            [FromQuery] FoodRequestDto? requestParams = null)
            => getItems(requestParams);

        [HttpPut("{itemId}")]
        public async Task<ActionResult> UpdateItem(int itemId, FoodItemEditsDto editsDto)
        {
            FoodItem? foodItem = await _repository.GetById(itemId);

            if (checkForAccessError(itemId, foodItem) is ActionResult accessError)
                return accessError;

            FoodItem checkedItem = foodItem!;

            if (editsDto.Description != null && editsDto.Description != checkedItem.Description)
                checkedItem.Description = editsDto.Description;

            if (editsDto.SugarMass.HasValue && editsDto.SugarMass != checkedItem.SugarMass)
                checkedItem.SugarMass = editsDto.SugarMass.Value;

            if (editsDto.WhenAdded.HasValue && editsDto.WhenAdded != checkedItem.WhenAdded)
                checkedItem.WhenAdded = editsDto.WhenAdded.Value;

            bool isUpdated = await _repository.Update(checkedItem);

            if (isUpdated)
                return NoContent();

            return Problem("Failed to update");
        }

        [HttpDelete("{itemId}")]
        public async Task<ActionResult> DeleteItem(int itemId)
        {
            FoodItem? foodItem = await _repository.GetById(itemId);

            if (checkForAccessError(itemId, foodItem) is ActionResult accessError)
                return accessError;

            bool result = await _repository.Delete(foodItem!);
            return result ? (ActionResult)NoContent() : Problem();
        }

        private async Task<ActionResult<PaginatedListDto<FoodItemDto>>> getItems(
            FoodRequestDto? requestParams, int? userId = null)
        {
            if (requestParams == null)
                requestParams = new FoodRequestDto();

            PaginatedList<FoodItem>? items = await _repository.GetList(requestParams.ToRefined(), userId);

            if (items == null)
                return Problem();
            else
                return items.ToDto(item => new FoodItemDto(item));
        }

        private bool isAssessForbidden(FoodItem foodItem)
        {
            return foodItem.UserInfoId != _requestContext.CurrentUser.Id && _requestContext.CurrentUser.Role != UserRole.Admin;
        }

        private ActionResult? checkForAccessError(int itemId, FoodItem? foodItem)
        {
            // we do not want to share information that resource exists, so, in any case return 404
            return foodItem == null || isAssessForbidden(foodItem)
                ? NotFound($"Item with id={itemId} is not found")
                : null;
        }

        private async Task<int> getSugarMass(FoodItemInputDto input)
        {
            return input.SugarMass
                ?? await _nutritionDataProvider.GetSugarInFood(input.Description.Trim())
                ?? 0;
        }
    }
}
