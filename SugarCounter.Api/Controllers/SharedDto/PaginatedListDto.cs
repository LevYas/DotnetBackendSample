using SugarCounter.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SugarCounter.Api.Controllers.SharedDto
{
    public class PaginatedListDto<TDto>
    {
        public PaginatedListDto()
        { }

        public PaginatedListDto(IEnumerable<TDto> items, PaginationInfo paginationInfo)
        {
            PageNumber = paginationInfo.PageNumber;
            ItemsPerPage = paginationInfo.ItemsPerPage;
            TotalPages = paginationInfo.TotalPages;
            TotalCount = paginationInfo.TotalCount;
            Items = items.ToList();
        }

        public List<TDto> Items { get; set; } = new List<TDto>();

        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    public static class PaginatedListDtoFactory
    {
        public static PaginatedListDto<TDto> ToDto<TModel, TDto>(this PaginatedList<TModel> list,
            Func<TModel, TDto> selector)
        {
            return new PaginatedListDto<TDto>(list.Items.Select(selector), list.PaginationInfo);
        }
    }
}
