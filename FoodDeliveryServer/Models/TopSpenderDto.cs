namespace FoodDeliveryServer.Models
{
    public class TopSpenderDto
    {
        // 必须是 public，否则外面的人看不见
        public int CustomerId { get; set; }
        public decimal TotalSpent { get; set; }
    }
}