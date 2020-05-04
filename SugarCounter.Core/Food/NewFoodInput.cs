using System;

namespace SugarCounter.Core.Food
{
    public class NewFoodInput
    {
        public NewFoodInput(int userId, string description, int sugarMass, DateTime whenAte)
        {
            UserId = userId;
            Description = description;
            SugarMass = sugarMass;
            WhenAdded = whenAte;
        }

        public int UserId { get; }
        public string Description { get; }
        public int SugarMass { get; }
        public DateTime WhenAdded { get; }
    }
}
