using System;

namespace SugarCounter.Core.Food
{
    public class FoodItem
    {
        public int Id { get; set; }
        public int UserInfoId { get; set; }
        public string Description { get; set; } = null!;
        public int SugarMass { get; set; }
        public DateTime WhenAdded { get; set; }
    }
}
