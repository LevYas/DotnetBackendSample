using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api;
using SugarCounter.Api.Controllers.Food;
using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Core.Food;
using SugarCounter.Core.Users;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerAddItemToUser
    {
        private readonly UserInfo _user = new UserInfo { Id = 1, Role = UserRole.User };

        private readonly Mock<INutritionDataProvider> _nutritionProviderMock = new Mock<INutritionDataProvider>();
        private readonly Mock<IFoodRepository> _repoMock = new Mock<IFoodRepository>();
        private readonly FoodController _controller;

        public FoodControllerAddItemToUser()
        {
            var userFood = new FoodItem() { Id = 1, UserInfoId = _user.Id, Description = "food" };

            _repoMock
                .Setup(r => r.Create(It.IsAny<NewFoodInput>()))
                .ReturnsAsync(userFood);

            var admin = new UserInfo { Id = 2, Role = UserRole.Admin };
            var adminContext = new RequestContext { CurrentUser = admin };

            _controller = new FoodController(_repoMock.Object, _nutritionProviderMock.Object, adminContext);
        }

        [Fact]
        public async Task RequestsSugarIfNotProvided()
        {
            const int sugar = 50;
            _nutritionProviderMock.Setup(r => r.GetSugarInFood(It.IsAny<string>()))
                .ReturnsAsync(sugar);
            FoodItemInputDto newFood = new FoodItemInputDto { Description = "food" };

            ActionResult<FoodItemDto> result = await _controller.AddItemToUser(_user.Id, newFood);

            _nutritionProviderMock.Verify(m => m.GetSugarInFood(It.IsAny<string>()));
            _repoMock.Verify(r => r.Create(It.Is<NewFoodInput>(f => f.SugarMass == sugar)));
        }

        [Fact]
        public async Task AssumesSugarIsZeroIfFailedToRetrieve()
        {
            _nutritionProviderMock.Setup(r => r.GetSugarInFood(It.IsAny<string>()))
                .ReturnsAsync((int?)null);
            FoodItemInputDto newFood = new FoodItemInputDto { Description = "food" };

            ActionResult<FoodItemDto> result = await _controller.AddItemToUser(_user.Id, newFood);

            _nutritionProviderMock.Verify(m => m.GetSugarInFood(It.IsAny<string>()));
            _repoMock.Verify(r => r.Create(It.Is<NewFoodInput>(f => f.SugarMass == 0)));
        }
    }
}
