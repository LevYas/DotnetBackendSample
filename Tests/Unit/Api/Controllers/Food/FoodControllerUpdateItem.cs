using Microsoft.AspNetCore.Mvc;
using Moq;
using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Core.Food;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerUpdateItem : FoodControllerTestsBase
    {
        public FoodControllerUpdateItem()
        {
            FoodRepoMock.Setup(r => r.GetById(UserFood.Id)).ReturnsAsync(UserFood);
            FoodRepoMock.Setup(r => r.Update(It.IsAny<FoodItem>())).ReturnsAsync(true);
        }

        [Fact]
        public async Task ReturnsNoContentForOwnItem()
        {
            ActionResult result = await FoodControllerForUser.UpdateItem(UserFood.Id, new FoodItemEditsDto());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ReturnsNoContentForAnyItemAsAdmin()
        {
            ActionResult result = await FoodControllerForAdmin.UpdateItem(UserFood.Id, new FoodItemEditsDto());

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenAccessForbidden()
        {
            ActionResult result = await FoodControllerForUser.UpdateItem(AnotherUserFood.Id, new FoodItemEditsDto());

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task UpdatesDescription()
        {
            var edits = new FoodItemEditsDto { Description = "Cheesecake" };

            var result = await FoodControllerForUser.UpdateItem(UserFood.Id, edits);

            Func<FoodItem, bool> isFoodItemOk = (fi) =>
            {
                return fi.Description == edits.Description
                    && fi.SugarMass == UserFood.SugarMass
                    && fi.WhenAdded == UserFood.WhenAdded;
            };
            FoodRepoMock.Verify(r => r.Update(It.Is<FoodItem>(fi => isFoodItemOk(fi))));
        }

        [Fact]
        public async Task UpdatesSugar()
        {
            var edits = new FoodItemEditsDto { SugarMass = UserFood.SugarMass + 1 };

            var result = await FoodControllerForUser.UpdateItem(UserFood.Id, edits);

            Func<FoodItem, bool> isFoodItemOk = (fi) =>
            {
                return fi.Description == UserFood.Description
                    && fi.SugarMass == edits.SugarMass
                    && fi.WhenAdded == UserFood.WhenAdded;
            };
            FoodRepoMock.Verify(r => r.Update(It.Is<FoodItem>(fi => isFoodItemOk(fi))));
        }

        [Fact]
        public async Task UpdatesDate()
        {
            var edits = new FoodItemEditsDto { WhenAdded = DateTime.Now };

            var result = await FoodControllerForUser.UpdateItem(UserFood.Id, edits);

            Func<FoodItem, bool> isFoodItemOk = (fi) =>
            {
                return fi.Description == UserFood.Description
                    && fi.SugarMass == UserFood.SugarMass
                    && fi.WhenAdded == edits.WhenAdded;
            };
            FoodRepoMock.Verify(r => r.Update(It.Is<FoodItem>(fi => isFoodItemOk(fi))));
        }
    }
}
