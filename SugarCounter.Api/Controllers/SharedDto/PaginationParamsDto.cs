using SugarCounter.Core.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace SugarCounter.Api.Controllers.SharedDto
{
    public class PaginationParamsDto
    {
        [Range(1, 10e6)]
        public int? PageNumber { get; set; }

        [Range(1, 100)]
        public int? ItemsPerPage { get; set; }

        public PaginationParams ToRefined()
        {
            return new PaginationParams
            {
                PageNumber = PageNumber ?? 1,
                ItemsPerPage = ItemsPerPage ?? 20,
            };
        }
    }
}
