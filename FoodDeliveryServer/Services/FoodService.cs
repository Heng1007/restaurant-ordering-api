using FoodDeliveryServer.Data;
using FoodDeliveryServer.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryServer.Services
{
    // 👨‍🍳 这是一个服务类 (Service)
    // 它负责具体的脏活累活（操作数据库）
    public class FoodService : IFoodService
    {
        private readonly AppDbContext _context;

        // 只有这里需要用到数据库！
        public FoodService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FoodItem>> GetAllFoods()
        {
            return await _context.FoodItems
                         .OrderBy(f => f.Id) // 或者是 f.Name
                         .Where(f => !f.IsDeleted)
                         .ToListAsync();
        }

        public async Task<FoodItem> AddFood(FoodItem food)
        {
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();

            return food;
        }

        public async Task DeleteFood(int d)
        {
           var food = await _context.FoodItems.FindAsync(d);
              if (food != null)
              {
                food.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<FoodItem?> GetFoodById(int id)
        {
            return await _context.FoodItems
                .Where(f => !f.IsDeleted)
                .FirstOrDefaultAsync(f => f.Id == id);
        }
    }
}