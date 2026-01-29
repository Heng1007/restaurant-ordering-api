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
            return await _context.FoodItems.Include(f => f.Id).ToListAsync();
        }

        public async Task AddFood(FoodItem food)
        {
            _context.FoodItems.Add(food);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteFood(int d)
        {
           var food = await _context.FoodItems.FindAsync(d);
              if (food != null)
              {
                _context.FoodItems.Remove(food);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<FoodItem?> GetFoodById(int id)
        {
            return await _context.FoodItems.FindAsync(id);
        }
    }
}