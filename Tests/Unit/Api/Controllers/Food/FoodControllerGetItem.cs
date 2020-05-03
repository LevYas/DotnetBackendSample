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
    public class FoodControllerGetItem
    {
        private readonly UserInfo _user = new UserInfo { Id = 1, Role = UserRole.User };
        private readonly UserInfo _admin = new UserInfo { Id = 2, Role = UserRole.Admin };

        private readonly RequestContext _userContext;
        private readonly RequestContext _adminContext;

        private readonly FoodItem _userFood;
        private readonly FoodItem _anotherUserFood;

        private readonly Mock<IFoodRepository> _repoMock = new Mock<IFoodRepository>();
        private FoodController _controller;

        public FoodControllerGetItem()
        {
            _userContext = new RequestContext { CurrentUser = _user };
            _adminContext = new RequestContext { CurrentUser = _admin };

            _userFood = new FoodItem() { Id = 1, UserInfoId = _user.Id, Description = "food" };
            _anotherUserFood = new FoodItem() { Id = 2, UserInfoId = 99, Description = "other food" };

            _controller = new FoodController(_repoMock.Object, new Mock<INutritionDataProvider>().Object, _userContext);
        }

        [Fact]
        public async Task ReturnsItem()
        {
            _repoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(_userFood);

            ActionResult<FoodItemDto> result = await _controller.GetItem(_userFood.Id);

            Assert.Equal(_userFood.Id, result.Value.Id);
            Assert.Equal(_userFood.Description, result.Value.Description);
        }

        [Fact]
        public async Task ReturnsNotFoundIfThereIsNoItem()
        {
            _repoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync((FoodItem?)null);

            ActionResult<FoodItemDto> result = await _controller.GetItem(1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ReturnsNotFoundIfItemOfAnotherUser()
        {
            _repoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(_anotherUserFood);

            ActionResult<FoodItemDto> result = await _controller.GetItem(1);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task ReturnsItemOfAnotherUserForAdmin()
        {
            _controller = new FoodController(_repoMock.Object, new Mock<INutritionDataProvider>().Object, _adminContext);
            _repoMock
                .Setup(r => r.GetById(It.IsAny<int>()))
                .ReturnsAsync(_userFood);

            ActionResult<FoodItemDto> result = await _controller.GetItem(_userFood.Id);

            Assert.Equal(_userFood.Id, result.Value.Id);
            Assert.Equal(_userFood.Description, result.Value.Description);
        }
    }
}
