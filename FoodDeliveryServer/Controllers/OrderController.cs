using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;
using FoodDeliveryServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. 获取所有订单
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var order = await _orderService.GetOrders();
            return Ok(order);
        }

        // 2. 下单 (Create Order)
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto dto)
        {
            var order = await _orderService.CreateOrder(dto);

            return Ok(order);
        }

        [HttpGet("TopSpender")]
        public async Task<ActionResult<TopSpenderDto>> GetTopSpender()
        {
            var result = await _orderService.GetTopSpender();
            if(result == null)
            {
                return NotFound("目前没有订单");
            }
            return Ok(result);
        }

    }
}