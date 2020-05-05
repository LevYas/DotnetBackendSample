using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Unit.Api.Controllers.Food
{
    public class FoodControllerGetTodaysSugar : FoodControllerTestsBase
    {
        [Fact]
        public async Task ReturnsSugarForOwner()
        {
            FoodRepoMock
                .Setup(r => r.GetTodaysSugar(User.Id))
                .ReturnsAsync(5);

            ActionResult<int> res = await FoodControllerForUser.GetTodaysSugar(User.Id);

            Assert.Equal(5, res.Value);
        }

        [Fact]
        public async Task ReturnsNotFoundForOtherUser()
        {
            ActionResult<int> res = await FoodControllerForUser.GetTodaysSugar(99);

            Assert.IsType<NotFoundResult>(res.Result);
        }

        [Fact]
        public async Task ReturnsSugarForAdmin()
        {
            FoodRepoMock
                .Setup(r => r.GetTodaysSugar(User.Id))
                .ReturnsAsync(5);

            ActionResult<int> res = await FoodControllerForAdmin.GetTodaysSugar(User.Id);

            Assert.Equal(5, res.Value);
        }
    }
}
