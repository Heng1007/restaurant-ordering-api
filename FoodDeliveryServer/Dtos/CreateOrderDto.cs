namespace FoodDeliveryServer.Dtos
{
    public class CreateOrderDto
    {
        public int UserId { get; set; }

        public string? CustomerNote { get; set; }

        public List<CreateOrderItemDto> Items { get; set; } = new();
    }

    public class CreateOrderItemDto
    {
        public int FoodItemId { get; set; }
        public int Quantity { get; set; }
    }
}
