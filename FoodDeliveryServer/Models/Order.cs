using System.Data;

namespace FoodDeliveryServer.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerName { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public int FoodItemId { get; set; }

        public FoodItem? FoodItem { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice
        {
            get
            {
                if(FoodItem == null) return 0;
                return FoodItem.Price * Quantity;
            }
        }
    }
}
