using SugarCounter.Api.Controllers.SharedDto;
using SugarCounter.Core.Food;
using System;

namespace SugarCounter.Api.Controllers.Food.Dto
{
    public class FoodRequestDto : PaginationParamsDto
    {
        public FoodRequestDto() { }

        public FoodRequestDto(int pageNumber, int itemsPerPage)
        {
            PageNumber = pageNumber;
            ItemsPerPage = itemsPerPage;
        }

        public DateTime? Day { get; set; }

        public new FoodRequest ToRefined()
            => new FoodRequest(base.ToRefined(), Day);
    }
}
