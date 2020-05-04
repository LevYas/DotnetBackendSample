using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SugarCounter.Core.Food;
using SugarCounter.Core.Shared;
using SugarCounter.DataAccess.Db;
using SugarCounter.DataAccess.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarCounter.DataAccess.Repositories
{
    internal class FoodRepository : IFoodRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<FoodRepository> _logger;

        public FoodRepository(AppDbContext context, ILogger<FoodRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FoodItem> Create(NewFoodInput foodInput)
        {
            var item = new FoodItem
            {
                UserInfoId = foodInput.UserId,
                Description = foodInput.Description,
                SugarMass = foodInput.SugarMass,
                WhenAdded = foodInput.WhenAdded
            };

            _context.FoodItems.Add(item);
            await _context.SaveChangesAsync();
            _context.Entry(item).State = EntityState.Detached;

            return item;
        }

        public async Task<bool> Delete(FoodItem item)
        {
            _context.FoodItems.Remove(item); // another option - just mark as deleted

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Task<FoodItem?> GetById(int itemId) => getItemById(itemId);

        public async Task<PaginatedList<FoodItem>?> GetList(FoodRequest request, int? userId = null)
        {
            IQueryable<FoodItem> collection = _context.FoodItems.AsNoTracking();

            if (userId != null)
                collection = collection.Where(i => i.UserInfoId == userId);

            try
            {
                int count = await collection.CountAsync();
                List<FoodItem> items = await collection.Paginate(request).ToListAsync();

                return new PaginatedList<FoodItem>(items, count, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve list of food items");
                return null;
            }
        }

        public async Task<bool> Update(FoodItem newValue)
        {
            FoodItem? oldValue = await getItemById(newValue.Id);

            if (oldValue == null)
                return false;

            _context.Entry(newValue).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return true; // How to improve: return false in case of exceptions
        }

        private async Task<FoodItem?> getItemById(int itemId)
            => await _context.FoodItems.AsNoTracking().SingleOrDefaultAsync(i => i.Id == itemId);
    }
}
