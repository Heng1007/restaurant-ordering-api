using FoodDeliveryServer.Data;
using FoodDeliveryServer.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryServer.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllOrders()
        {
            return await _context.Orders
                .Include(o => o.FoodItem)
                .ToListAsync();
        }

        public async Task CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task<TopSpenderDto?> GetTopSpender()
        {
            var topSpender = await _context.Orders
                .Include(o => o.FoodItem)
                .GroupBy(o => o.CustomerName)
                .Select(g => new TopSpenderDto
                {
                    CustomerName = g.Key,
                    TotalSpent = g.Sum(o => o.Quantity * o.FoodItem!.Price)
                })
                .OrderByDescending(x => x.TotalSpent)
                .FirstOrDefaultAsync(); // 👈 直接拿第1个，拿不到就是 null
            return topSpender;
        }
    }

}
