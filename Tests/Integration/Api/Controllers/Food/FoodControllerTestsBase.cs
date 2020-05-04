using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using SugarCounter.Api.Controllers.Food;
using SugarCounter.Api.Controllers.Food.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Integration.Api.Controllers.Food
{
    public class FoodControllerTestsBase : IntegrationTestsBase
    {
        protected readonly FoodController Controller;

        public FoodControllerTestsBase()
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> {
                    { "NutritionixKeys:ServerUrl", "https://nutritionix_url_here.com/" },
                });
            var nutritionixClient = new NutritionixClient(builder.Build(), new HttpClient(), new NullLogger<NutritionixClient>());

            Controller = new FoodController(FoodRepo, nutritionixClient, AdminContext);
        }

        protected FoodItemInputDto CreateFoodInput(int? sugar = null, DateTime? whenAdded = null, string? descr = null)
        {
            return new FoodItemInputDto
            {
                Description = descr ?? "Quiche",
                SugarMass = sugar ?? 50,
                WhenAdded = whenAdded ?? DateTime.Now
            };
        }

        protected async Task<int> AddItemToRoot(int? calories = null, DateTime? whenAdded = null, string? descr = null)
        {
            FoodItemDto item = await AddItemToRootAndReturnIt(calories, whenAdded, descr);
            return item.Id;
        }

        protected async Task<FoodItemDto> AddItemToRootAndReturnIt(int? calories = null, DateTime? whenAdded = null,
            string? descr = null)
        {
            ActionResult<FoodItemDto> addedItem = await Controller.AddItemToUser(1, CreateFoodInput(calories, whenAdded, descr));
            return addedItem.Value;
        }

        protected async Task<int[]> AddItemsToRoot(int count)
        {
            List<Task<int>> tasks = new List<Task<int>>();

            for (int i = 0; i < count; i++)
                tasks.Add(AddItemToRoot());

            await Task.WhenAll(tasks);

            return tasks.Select(t => t.Result).ToArray();
        }
    }
}
