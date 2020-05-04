using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Api.Controllers.SharedDto;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Functional.Food
{
    public class PaginationTests : ApiTestsBase
    {
        public PaginationTests(CustomWebApplicationFactory factory) : base(factory)
        {
            AuthApi.LoginAsRoot().Wait();
            prepareItems(15).Wait();
        }

        [Fact]
        public async Task ShowsOnlyDesiredNumberOfCurrentUserItems()
        {
            var result = await FoodApi.GetItemsOfCurrentUser(1, 10);

            Assert.Equal(10, result.Items.Count);
        }

        [Fact]
        public async Task ShowsLeftCurrentUserItemsOnNextPage()
        {
            PaginatedListDto<FoodItemDto> allItems = await FoodApi.GetItemsOfCurrentUser(1, 100);
            int pageSize = allItems.Items.Count - 5;

            var result = await FoodApi.GetItemsOfCurrentUser(2, pageSize);

            Assert.Equal(5, result.Items.Count);
        }

        [Fact]
        public async Task ShowsOnlyDesiredNumberOfAllItems()
        {
            var result = await FoodApi.GetAllItems(1, 10);

            Assert.Equal(10, result.Items.Count);
        }

        [Fact]
        public async Task ReturnsEmptyCollectionIfNoMoreResults()
        {
            var result = await FoodApi.GetAllItems(20, 10);

            Assert.Empty(result.Items);
        }

        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(1, 101)]
        public async Task ReturnsErrorOnIncorrectNumbers(int pageNumber, int itemsPerPage)
        {
            var response = await FoodApi.SendGetAllItems(pageNumber, itemsPerPage);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task prepareItems(int count)
        {
            for (int i = 0; i < count; i++)
                await FoodApi.AddItemForCurrentUser(FoodApi.CreateNewFoodInput($"{i} apple", i));
        }
    }
}
