using System;
using System.Collections.Generic;

namespace SugarCounter.Core.Shared
{
    public class PaginatedList<T>
    {
        public PaginatedList(List<T> items, int count, PaginationParams requestParams)
        {
            Items = items;

            PaginationInfo = new PaginationInfo
            {
                PageNumber = requestParams.PageNumber,
                TotalCount = count,
                ItemsPerPage = requestParams.ItemsPerPage,
                TotalPages = (int)Math.Ceiling(count / (double)requestParams.ItemsPerPage)
            };
        }

        public List<T> Items { get; }
        public PaginationInfo PaginationInfo { get; }
    }

    public struct PaginationInfo
    {
        public int PageNumber { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }
}
