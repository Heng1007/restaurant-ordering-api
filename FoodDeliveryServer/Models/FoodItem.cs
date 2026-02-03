namespace FoodDeliveryServer.Models
{
    public class FoodItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
