using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Api.Controllers.Users.Dto;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Food
{
    public class AdminActionsTests : ApiTestsBase
    {
        public AdminActionsTests(CustomWebApplicationFactory factory) : base(factory)
        {
            AuthApi.LoginAsRoot().Wait();
        }

        [Fact]
        public async Task CanAddItemToOtherUser()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            var newItem = FoodApi.CreateNewFoodInput();

            FoodItemDto result = await FoodApi.AddItemForUser(newUser.Id, newItem);

            Assert.True(result.Id > 0);
            Assert.Equal(newItem.Description, result.Description);
            Assert.Equal(newItem.SugarMass, result.SugarMass);
            Assert.Equal(newItem.WhenAdded, result.WhenAdded);
        }

        [Fact]
        public async Task CanGetAnyItem()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            FoodItemInputDto newItem = FoodApi.CreateNewFoodInput();
            FoodItemDto addedItem = await FoodApi.AddItemForUser(newUser.Id, newItem);

            var result = await FoodApi.GetItem(addedItem.Id);

            Assert.Equal(addedItem.Id, result.Id);
        }

        [Fact]
        public async Task CanGetAnySugar()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            FoodItemInputDto newItem = FoodApi.CreateNewFoodInput(sugar: 50000);
            await FoodApi.AddItemForUser(newUser.Id, newItem);

            int result = await FoodApi.GetTodaysSugar(newUser.Id);

            Assert.True(result >= 50000);
        }

        [Fact]
        public async Task CanGetItemsOfAnyUser()
        {
            await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput());
            UserInfoDto newUser = await UsersApi.CreateUser();
            await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput());
            await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput("Apple"));
            await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput("Cheesecake"));

            var result = await FoodApi.GetItemsOfUser(newUser.Id);

            Assert.Equal(3, result.Items.Count);
        }

        [Fact]
        public async Task CanGetAllItems()
        {
            string key = "AllItems " + DateTime.Now.Ticks;
            await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput(key));
            UserInfoDto newUser = await UsersApi.CreateUser();
            await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput(key));
            await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput(key));

            var result = await FoodApi.GetAllItems();

            Assert.Equal(3, result.Items.Count(i => i.Description.Contains(key)));
        }

        [Fact]
        public async Task CanUpdateAnyItem()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            FoodItemDto addedItem = await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput("Apple"));
            var edits = new FoodItemEditsDto { Description = "Cheesecake", SugarMass = 999 };

            await FoodApi.UpdateItem(addedItem.Id, edits);

            FoodItemDto result = await FoodApi.GetItem(addedItem.Id);
            Assert.Equal(edits.Description, result.Description);
            Assert.Equal(edits.SugarMass, result.SugarMass);
        }

        [Fact]
        public async Task CanDeleteAnyItem()
        {
            UserInfoDto newUser = await UsersApi.CreateUser();
            FoodItemDto addedItem = await FoodApi.AddItemForUser(newUser.Id, FoodApi.CreateNewFoodInput());

            await FoodApi.DeleteItem(addedItem.Id);

            HttpResponseMessage response = await FoodApi.SendGetItem(addedItem.Id);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
