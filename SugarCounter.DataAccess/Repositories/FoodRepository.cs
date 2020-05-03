using SugarCounter.Core.Food;
using SugarCounter.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarCounter.DataAccess.Repositories
{
    internal class FoodRepository : IFoodRepository
    {
        public Task<FoodItem> Create(NewFoodInput foodInput)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(FoodItem item)
        {
            throw new NotImplementedException();
        }

        public Task<FoodItem?> GetById(int itemId)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<FoodItem>?> GetList(FoodRequest request, int? userId = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(FoodItem newValue)
        {
            throw new NotImplementedException();
        }
    }
}
