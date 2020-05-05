using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Core.Food;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerGetItem : FoodControllerTestsBase
    {
        [Fact]
        public async Task ReturnsItem()
        {
            FoodRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(UserFood);

            ActionResult<FoodItemDto> result = await FoodControllerForUser.GetItem(UserFood.Id);

            Assert.Equal(UserFood.Id, result.Value.Id);
            Assert.Equal(UserFood.Description, result.Value.Description);
        }

        [Fact]
        public async Task ReturnsNotFoundIfThereIsNoItem()
        {
            FoodRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync((FoodItem?)null);

            ActionResult<FoodItemDto> result = await FoodControllerForUser.GetItem(1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ReturnsNotFoundIfItemOfAnotherUser()
        {
            FoodRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(AnotherUserFood);

            ActionResult<FoodItemDto> result = await FoodControllerForUser.GetItem(1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ReturnsItemOfAnotherUserForAdmin()
        {
            FoodRepoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(UserFood);

            ActionResult<FoodItemDto> result = await FoodControllerForAdmin.GetItem(UserFood.Id);

            Assert.Equal(UserFood.Id, result.Value.Id);
            Assert.Equal(UserFood.Description, result.Value.Description);
        }
    }
}
