using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Core.Food;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerAddItemToUser : FoodControllerTestsBase
    {
        public FoodControllerAddItemToUser()
        {
            FoodRepoMock.Setup(r => r.Create(It.IsAny<NewFoodInput>())).ReturnsAsync(UserFood);
        }

        [Fact]
        public async Task RequestsSugarIfNotProvided()
        {
            const int sugar = 50;
            NutritionProviderMock.Setup(r => r.GetSugarInFood(It.IsAny<string>())).ReturnsAsync(sugar);
            FoodItemInputDto newFood = new FoodItemInputDto { Description = "food" };

            ActionResult<FoodItemDto> result = await FoodControllerForAdmin.AddItemToUser(User.Id, newFood);

            NutritionProviderMock.Verify(m => m.GetSugarInFood(It.IsAny<string>()));
            FoodRepoMock.Verify(r => r.Create(It.Is<NewFoodInput>(f => f.SugarMass == sugar)));
        }

        [Fact]
        public async Task AssumesSugarIsZeroIfFailedToRetrieve()
        {
            NutritionProviderMock.Setup(r => r.GetSugarInFood(It.IsAny<string>())).ReturnsAsync((int?)null);
            FoodItemInputDto newFood = new FoodItemInputDto { Description = "food" };

            ActionResult<FoodItemDto> result = await FoodControllerForAdmin.AddItemToUser(User.Id, newFood);

            NutritionProviderMock.Verify(m => m.GetSugarInFood(It.IsAny<string>()));
            FoodRepoMock.Verify(r => r.Create(It.Is<NewFoodInput>(f => f.SugarMass == 0)));
        }
    }
}
