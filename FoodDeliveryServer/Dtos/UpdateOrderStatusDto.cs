using FoodDeliveryServer.Models;
using System.ComponentModel.DataAnnotations;

namespace FoodDeliveryServer.Dtos
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
