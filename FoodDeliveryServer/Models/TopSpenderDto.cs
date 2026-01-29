namespace FoodDeliveryServer.Models
{
    public class TopSpenderDto
    {
        // 必须是 public，否则外面的人看不见
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
    }
}