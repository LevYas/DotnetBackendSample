using SugarCounter.Core.Food;
using System;
using System.ComponentModel.DataAnnotations;

namespace SugarCounter.Api.Controllers.Food.Dto
{
    public class FoodItemInputDto
    {
        [StringLength(FoodItemLimits.MaxDescriptionLength, MinimumLength = FoodItemLimits.MinDescriptionLength)]
        public string Description { get; set; } = default!;

        public int? SugarMass { get; set; }
        public DateTime? WhenAdded { get; set; }
    }
}