using System.Threading.Tasks;

namespace SugarCounter.Api.Controllers.Food
{
    public interface INutritionDataProvider
    {
        Task<int?> GetSugarInFood(string foodDescription);
    }
}
