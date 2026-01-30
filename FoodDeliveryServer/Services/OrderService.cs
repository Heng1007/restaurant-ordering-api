using FoodDeliveryServer.Data;
using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryServer.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IAIService _aiService;

        public OrderService(AppDbContext context, IAIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<List<Order>> GetOrders()
        {
            return await _context.Orders.Include(o => o.Items).ThenInclude(i => i.Food).ToListAsync();
        }

        public async Task<Order> CreateOrder(CreateOrderDto dto)
        {
            decimal totalPrice = 0;
            var finalOrderItems = new List<OrderItem>();

            foreach(var itemDto in dto.Items)
            {
                var food = await _context.FoodItems.FindAsync(itemDto.FoodItemId);

                if (food == null) continue;

                totalPrice += food.Price * itemDto.Quantity;

                var orderItem = new OrderItem
                {
                    FoodId = food.Id,
                    Price = food.Price,
                    Quantity = itemDto.Quantity,
                };

                finalOrderItems.Add(orderItem);
            }

            string sentimentResult = "Neutral";
            if (!string.IsNullOrEmpty(dto.CustomerNote))
            {
                // 如果顾客写了备注，就发给 AI 看看
                sentimentResult = await _aiService.AnalyzeSentiment(dto.CustomerNote);
            }

            var order = new Order
            {
                UserId = dto.UserId,
                OrderDate = DateTime.Now,
                CustomerNote = dto.CustomerNote,
                TotalPrice = totalPrice,
                Sentiment = sentimentResult,
                Items = finalOrderItems,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<TopSpenderDto?> GetTopSpender()
        {
            var topSpender = await _context.Orders
                .GroupBy(o => o.UserId)
                .Select(g => new TopSpenderDto
                {
                    CustomerId = g.Key,
                    TotalSpent = g.Sum(o => o.TotalPrice)
                })
                .OrderByDescending(x => x.TotalSpent)
                .FirstOrDefaultAsync(); // 👈 直接拿第1个，拿不到就是 null
            return topSpender;
        }
    }

}
