namespace FoodDeliveryServer.Models
{
    public class Order
    {
        public int Id { get; set; }

        // 如果你有User表，最好存 UserId；如果没有，先保留 CustomerName
        public int UserId { get; set; }
        // public string CustomerName { get; set; } // (旧的，建议换成 UserId)

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // 👇 修正：钱必须是存下来的死数字，不能是用 get 计算的
        public decimal TotalPrice { get; set; }

        // 👇 新增：顾客备注
        public string? CustomerNote { get; set; }

        // 👇 新增：AI 分析结果
        public string? Sentiment { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // 👇 关键：一个订单包含“一堆”具体的菜
        public List<OrderItem> Items { get; set; } = new();
    }
}