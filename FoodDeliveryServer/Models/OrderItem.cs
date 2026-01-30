using System.Text.Json.Serialization;

namespace FoodDeliveryServer.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; } // 买了几个
        public decimal Price { get; set; } // 当时的单价 (防止涨价影响)

        // 关系：属于哪个主订单
        public int OrderId { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }

        // 关系：是哪种食物
        public int FoodId { get; set; }   // 对应你的 FoodItem 表
        public FoodItem? Food { get; set; }
    }
}