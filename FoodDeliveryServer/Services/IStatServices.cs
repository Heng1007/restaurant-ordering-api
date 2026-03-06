using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryServer.Services
{
    public interface IStatServices
    {
        Task<object> GetBestSellers();
        Task<object> GetTopSpender();
        Task<object> GetSentimentStats();
    }
}
