using SugarCounter.Api.Controllers.Food.Dto;
using SugarCounter.Api.Controllers.SharedDto;
using System.Threading.Tasks;
using Xunit;

namespace Integration.Api.Controllers.Food
{
    public class FoodControllerGetAllItems : FoodControllerTestsBase
    {
        [Fact]
        public async Task ProvidesCorrectPaginationInfo()
        {
            await prepareItems(15);

            var result = await Controller.GetAllItems(new FoodRequestDto(1, 10));

            PaginatedListDto<FoodItemDto> list = result.Value;
            Assert.Equal(10, list.ItemsPerPage);
            Assert.Equal(1, list.PageNumber);
            Assert.Equal(15, list.TotalCount);
            Assert.Equal(2, list.TotalPages);
        }

        private async Task prepareItems(int count)
        {
            for (int i = 0; i < count; i++)
                await Controller.AddItem(new FoodItemInputDto { Description = $"{i} apples", SugarMass = i * 10 });
        }
    }
}
