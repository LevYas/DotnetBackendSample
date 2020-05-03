using SugarCounter.Core.Shared;
using System.Threading.Tasks;

namespace SugarCounter.Core.Food
{
    public interface IFoodRepository
    {
        Task<FoodItem> Create(NewFoodInput foodInput);
        Task<PaginatedList<FoodItem>?> GetList(FoodRequest request, int? userId = null);
        Task<FoodItem?> GetById(int itemId);
        Task<bool> Update(FoodItem newValue);
        Task<bool> Delete(FoodItem item);
    }
}
