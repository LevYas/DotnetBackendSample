using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api;
using SugarCounter.Api.Controllers.Food;
using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Core.Food;
using SugarCounter.Core.Users;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerUpdateItem
    {
        private readonly UserInfo _user = new UserInfo { Id = 1, Role = UserRole.User };
        private readonly UserInfo _admin = new UserInfo { Id = 2, Role = UserRole.Admin };

        private readonly RequestContext _userContext;
        private readonly RequestContext _adminContext;

        private readonly FoodItem _userFood;
        private readonly FoodItem _anotherUserFood;

        private readonly Mock<INutritionDataProvider> _nutritionProviderMock = new Mock<INutritionDataProvider>();
        private readonly Mock<IFoodRepository> _repoMock = new Mock<IFoodRepository>();
        private FoodController _controller;

        public FoodControllerUpdateItem()
        {
            _userContext = new RequestContext { CurrentUser = _user };
            _adminContext = new RequestContext { CurrentUser = _admin };

            _userFood = new FoodItem() { Id = 1, UserInfoId = _user.Id, Description = "food" };
            _anotherUserFood = new FoodItem() { Id = 2, UserInfoId = 99, Description = "other food" };

            _repoMock
                .Setup(r => r.GetById(_userFood.Id))
                .ReturnsAsync(_userFood);

            _repoMock
                .Setup(r => r.Update(It.IsAny<FoodItem>()))
                .ReturnsAsync(true);

            _controller = new FoodController(_repoMock.Object, _nutritionProviderMock.Object, _userContext);
        }

        [Fact]
        public async Task ReturnsNoContentForOwnItem()
        {
            ActionResult result = await _controller.UpdateItem(_userFood.Id, new FoodItemEditsDto());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ReturnsNoContentForAnyItemAsAdmin()
        {
            _controller = new FoodController(_repoMock.Object, new Mock<INutritionDataProvider>().Object, _adminContext);

            ActionResult result = await _controller.UpdateItem(_userFood.Id, new FoodItemEditsDto());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenAccessForbidden()
        {
            ActionResult result = await _controller.UpdateItem(_anotherUserFood.Id, new FoodItemEditsDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdatesDescription()
        {
            var edits = new FoodItemEditsDto { Description = "Cheesecake" };

            var result = await _controller.UpdateItem(_userFood.Id, edits);

            Func<FoodItem, bool> isFoodItemOk = (fi) =>
            {
                return fi.Description == edits.Description
                    && fi.SugarMass == _userFood.SugarMass
                    && fi.WhenAdded == _userFood.WhenAdded;
            };
            _repoMock.Verify(r => r.Update(It.Is<FoodItem>(fi => isFoodItemOk(fi))));
        }

        [Fact]
        public async Task UpdatesSugar()
        {
            var edits = new FoodItemEditsDto { SugarMass = _userFood.SugarMass + 1 };

            var result = await _controller.UpdateItem(_userFood.Id, edits);

            Func<FoodItem, bool> isFoodItemOk = (fi) =>
            {
                return fi.Description == _userFood.Description
                    && fi.SugarMass == edits.SugarMass
                    && fi.WhenAdded == _userFood.WhenAdded;
            };
            _repoMock.Verify(r => r.Update(It.Is<FoodItem>(fi => isFoodItemOk(fi))));
        }

        [Fact]
        public async Task UpdatesDate()
        {
            var edits = new FoodItemEditsDto { WhenAdded = DateTime.Now };

            var result = await _controller.UpdateItem(_userFood.Id, edits);

            Func<FoodItem, bool> isFoodItemOk = (fi) =>
            {
                return fi.Description == _userFood.Description
                    && fi.SugarMass == _userFood.SugarMass
                    && fi.WhenAdded == edits.WhenAdded;
            };
            _repoMock.Verify(r => r.Update(It.Is<FoodItem>(fi => isFoodItemOk(fi))));
        }
    }
}
