using System.Text.Json.Serialization;

namespace FoodDeliveryServer.Models
{

    // 👇 这个 Attribute 很关键！它告诉 Swagger 把 0,1,2 变成 "Pending", "InProgress"...
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderStatus
    {
        Pending,      // 待处理
        InProgress,   // 制作中
        Delivered,    // 已送达
        Cancelled     // 已取消
    }
}