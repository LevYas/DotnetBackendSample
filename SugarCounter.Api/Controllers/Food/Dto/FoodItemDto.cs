using SugarCounter.Core.Food;
using System;

namespace SugarCounter.Api.Controllers.Food.Dto
{
    public class FoodItemDto
    {
        [Obsolete("For serializer only")]
        public FoodItemDto() { }

        public FoodItemDto(FoodItem foodItem)
        {
            Id = foodItem.Id;
            Description = foodItem.Description;
            SugarMass = foodItem.SugarMass;
            WhenAdded = foodItem.WhenAdded;
        }

        public int Id { get; set; }

        public string Description { get; set; } = default!;
        public int SugarMass { get; set; }
        public DateTime WhenAdded { get; set; }
    }
}
