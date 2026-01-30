using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;

namespace FoodDeliveryServer.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetOrders();
        Task<Order> CreateOrder(CreateOrderDto order);

        Task<TopSpenderDto?> GetTopSpender();
    }
}
