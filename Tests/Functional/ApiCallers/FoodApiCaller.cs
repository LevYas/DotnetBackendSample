using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Api.Controllers.SharedDto;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Functional.ApiCallers
{
    public class FoodApiCaller : ApiCaller
    {
        public const string FoodUrl = "/api/v1/food";

        public FoodApiCaller(HttpClient client) : base(client) { }

        public FoodItemInputDto CreateNewFoodInput(string desc = "Pie", int sugar = 50, DateTime? whenAdded = null)
        {
            return new FoodItemInputDto
            {
                Description = desc,
                SugarMass = sugar,
                WhenAdded = whenAdded ?? DateTime.Now
            };
        }

        public async Task<FoodItemDto> AddItemForCurrentUser(FoodItemInputDto newItem)
            => await ReadResponse<FoodItemDto>(SendAddItemForCurrentUser(newItem));

        public async Task<HttpResponseMessage> SendAddItemForCurrentUser(FoodItemInputDto newItem)
            => await Client.PostAsJsonAsync(FoodUrl, newItem);

        public async Task<FoodItemDto> AddItemForUser(int userId, FoodItemInputDto newItem)
            => await ReadResponse<FoodItemDto>(SendAddItemForUser(userId, newItem));

        public async Task<HttpResponseMessage> SendAddItemForUser(int userId, FoodItemInputDto newItem)
            => await Client.PostAsJsonAsync($"{FoodUrl}/forUser/{userId}", newItem);

        public Task<FoodItemDto> GetItem(int id) => ReadResponse<FoodItemDto>(SendGetItem(id));
        public async Task<HttpResponseMessage> SendGetItem(int id) => await Client.GetAsync($"{FoodUrl}/{id}");

        public Task<PaginatedListDto<FoodItemDto>> GetItemsOfCurrentUser(int? pageNumber = null, int? itemsPerPage = null)
        {
            return ReadResponse<PaginatedListDto<FoodItemDto>>(SendGetItemsOfCurrentUser(pageNumber, itemsPerPage));
        }

        public async Task<HttpResponseMessage> SendGetItemsOfCurrentUser(int? pageNumber = null, int? itemsPerPage = null)
        {
            return await Client.GetAsync(CombineUrl($"{FoodUrl}/myItems", pageNumber, itemsPerPage));
        }

        public Task<PaginatedListDto<FoodItemDto>> GetItemsOfUser(int id)
            => ReadResponse<PaginatedListDto<FoodItemDto>>(SendGetItemsOfUser(id));

        public async Task<HttpResponseMessage> SendGetItemsOfUser(int id)
            => await Client.GetAsync($"{FoodUrl}/ofUser/{id}");

        public Task<PaginatedListDto<FoodItemDto>> GetAllItems(int? pageNumber = null, int? itemsPerPage = null)
        {
            return ReadResponse<PaginatedListDto<FoodItemDto>>(SendGetAllItems(pageNumber, itemsPerPage));
        }

        public async Task<HttpResponseMessage> SendGetAllItems(int? pageNumber = null, int? itemsPerPage = null)
        {
            return await Client.GetAsync(CombineUrl(FoodUrl, pageNumber, itemsPerPage));
        }

        public Task UpdateItem(int id, FoodItemEditsDto dto) => EnsureSuccess(SendUpdateItem(id, dto));

        public async Task<HttpResponseMessage> SendUpdateItem(int id, FoodItemEditsDto dto)
            => await Client.PutAsJsonAsync($"{FoodUrl}/{id}", dto);

        public Task DeleteItem(int id) => EnsureSuccess(SendDeleteItem(id));
        public async Task<HttpResponseMessage> SendDeleteItem(int id) => await Client.DeleteAsync($"{FoodUrl}/{id}");
    }
}
