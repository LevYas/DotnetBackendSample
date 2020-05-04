using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Api.Controllers.Users.Dto;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Food
{
    public class UserActionsTests : ApiTestsBase
    {
        private readonly UserInfoDto _user;

        public UserActionsTests(CustomWebApplicationFactory factory) : base(factory)
        {
            Task<UserInfoDto> newUserTask = UsersApi.CreateUser();
            newUserTask.Wait();
            _user = newUserTask.Result;

            AuthApi.Login(_user.Login).Wait();
        }

        [Fact]
        public async Task CanAddItem()
        {
            FoodItemInputDto newItem = FoodApi.CreateNewFoodInput();

            FoodItemDto result = await FoodApi.AddItemForCurrentUser(newItem);

            Assert.True(result.Id > 0);
            Assert.Equal(newItem.Description, result.Description);
            Assert.Equal(newItem.SugarMass, result.SugarMass);
            Assert.Equal(newItem.WhenAdded, result.WhenAdded);
        }

        [Fact]
        public async Task CanNotAddItemToOtherUser()
        {
            HttpResponseMessage response = await FoodApi.SendAddItemForUser(_user.Id, FoodApi.CreateNewFoodInput());

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CanGetOwnedItem()
        {
            FoodItemInputDto newItem = FoodApi.CreateNewFoodInput();
            FoodItemDto addedItem = await FoodApi.AddItemForCurrentUser(newItem);

            FoodItemDto result = await FoodApi.GetItem(addedItem.Id);

            Assert.Equal(addedItem.Id, result.Id);
        }

        [Fact]
        public async Task CanGetOwnedItems()
        {
            await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput());
            await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput("Apple"));
            await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput("Cheesecake"));

            PaginatedListDto<FoodItemDto> result = await FoodApi.GetItemsOfCurrentUser();

            Assert.Equal(3, result.Items.Count);
        }

        [Fact]
        public async Task ApiDoesNotAcceptInvalidDescriptionOnUpdate()
        {
            FoodItemDto addedItem = await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput("Apple"));
            var edits = new FoodItemEditsDto { Description = "d" };

            var response = await FoodApi.SendUpdateItem(addedItem.Id, edits);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ApiDoesNotAcceptInvalidDescriptionOnAdd()
        {
            FoodItemInputDto newFood = new FoodItemInputDto { Description = "d", SugarMass = 10 };

            var response = await FoodApi.SendAddItemForCurrentUser(newFood);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CanUpdateOwnedItem()
        {
            FoodItemDto addedItem = await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput("Apple"));
            var edits = new FoodItemEditsDto { Description = "Cheesecake", SugarMass = 999 };

            await FoodApi.UpdateItem(addedItem.Id, edits);

            FoodItemDto result = await FoodApi.GetItem(addedItem.Id);
            Assert.Equal(edits.Description, result.Description);
            Assert.Equal(edits.SugarMass, result.SugarMass);
        }

        [Fact]
        public async Task CanDeleteOwnedItem()
        {
            FoodItemDto addedItem = await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput());

            await FoodApi.DeleteItem(addedItem.Id);

            HttpResponseMessage response = await FoodApi.SendGetItem(addedItem.Id);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
