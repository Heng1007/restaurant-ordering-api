using FoodDeliveryServer.Models;

namespace FoodDeliveryServer.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrders();
        Task CreateOrder(Order order);

        Task<TopSpenderDto?> GetTopSpender();
    }
}
