using SugarCounter.Core.Shared;
using System;

namespace SugarCounter.Core.Food
{
    public class FoodRequest : PaginationParams
    {
        public FoodRequest() { }

        public FoodRequest(PaginationParams paginationParams, DateTime? day)
        {
            PageNumber = paginationParams.PageNumber;
            ItemsPerPage = paginationParams.ItemsPerPage;
            Day = day;
        }

        public DateTime? Day { get; set; }
    }
}
