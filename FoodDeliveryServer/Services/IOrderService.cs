using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;

namespace FoodDeliveryServer.Services
{
    public interface IOrderService
    {
        Task<PagedResult<Order>> GetOrdersAsync(int page, int pageSize);
        Task<List<Order>> GetOrders();
        Task<List<Order>> GetOrdersByUserId(int userId);
        Task<Order> CreateOrder(int userId, CreateOrderDto order);
        Task SetOrderStatus(int orderId, OrderStatus status);
        Task<string?> CancelOrder(int orderId, int userId);

        Task<TopSpenderDto?> GetTopSpender();
    }
}
