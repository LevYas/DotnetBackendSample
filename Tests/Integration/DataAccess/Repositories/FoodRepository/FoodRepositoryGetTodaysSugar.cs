using SugarCounter.Core.Food;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Integration.DataAccess.Repositories.FoodRepository
{
    public class FoodRepositoryGetTodaysSugar : IntegrationTestsBase
    {
        [Fact]
        public async Task ReturnsZeroIfNoElements()
        {
            int res = await FoodRepo.GetTodaysSugar(1);

            Assert.Equal(0, res);
        }

        [Fact]
        public async Task ReturnsSumOfSugar()
        {
            await FoodRepo.Create(new NewFoodInput(1, "", 10, DateTime.Now));
            await FoodRepo.Create(new NewFoodInput(1, "", 25, DateTime.Now));
            await FoodRepo.Create(new NewFoodInput(2, "", 25, DateTime.Now));
            await FoodRepo.Create(new NewFoodInput(1, "", 25, DateTime.Now.AddDays(-1)));

            int res = await FoodRepo.GetTodaysSugar(1);

            Assert.Equal(35, res);
        }
    }
}
