using Microsoft.AspNetCore.Mvc;
using SugarCounter.Api.Controllers.Food.Dto;
using System;
using System.Linq;
using Xunit;

namespace Integration.Api.Controllers.Food
{
    public class FoodControllerAddItem : FoodControllerTestsBase
    {
        [Fact]
        public async void SavesItemToDb()
        {
            var foodInput = new FoodItemInputDto
            {
                Description = "Apple",
                SugarMass = 100,
                WhenAdded = DateTime.Now
            };

            ActionResult<FoodItemDto> itemDto = await Controller.AddItemToUser(1, foodInput);

            var item = DbContext.FoodItems.Single(i => i.Id == itemDto.Value.Id);

            Assert.Equal(foodInput.Description, item.Description);
            Assert.Equal(foodInput.SugarMass, item.SugarMass);
            Assert.Equal(foodInput.WhenAdded, item.WhenAdded);
        }
    }
}
