using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;
using FoodDeliveryServer.Services;
using FoodDeliveryServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        private int CurrentUserId
        {
            get
            {
                var idClaim =  User.FindFirst("id")?.Value;
                if (string.IsNullOrEmpty(idClaim) || !int.TryParse(idClaim, out int userId))
                {
                    // 如果拿不到 ID，抛个异常或者返回 0 (视情况而定)
                    // 这里为了简单，如果拿不到通常说明 [Authorize] 没生效或 Token 坏了
                    return 0;
                }
                return userId;
            }
        }

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. 获取所有订单
        [Authorize(Roles = Constants.Roles.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            var order = await _orderService.GetOrders();
            return Ok(order);
        }

        [Authorize]
        [HttpGet("MyOrders")]
        public async Task<ActionResult<List<Order>>> GetMyOrders()
        {  
            if(CurrentUserId == 0)
            {
                return Unauthorized("无法识别用户身份");
            }
            var orders = await _orderService.GetOrdersByUserId(CurrentUserId);
            return Ok(orders);
        }


        // 2. 下单 (Create Order)
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(CreateOrderDto dto)
        {
            if (CurrentUserId == 0)
            {
                return Unauthorized("无法识别用户身份");
            }
            var order = await _orderService.CreateOrder(CurrentUserId, dto);

            return Ok(order);
        }

        [HttpGet("TopSpender")]
        public async Task<ActionResult<TopSpenderDto>> GetTopSpender()
        {
            var result = await _orderService.GetTopSpender();
            if (result == null)
            {
                return NotFound("目前没有订单");
            }
            return Ok(result);
        }


        [Authorize(Roles = Constants.Roles.Admin)]
        [HttpGet("PagedResult")]
        // 👇 [FromQuery] 表示参数来自网址问号后面 (例如 ?page=2)
        public async Task<ActionResult<PagedResult<Order>>> GetOrders(
            [FromQuery] int page = 1,      // 默认第 1 页
            [FromQuery] int pageSize = 10  // 默认每页 10 条
        )
        {
            // 防御性编程：防止有人传 page=-1 或者 pageSize=1000000
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            var result = await _orderService.GetOrdersAsync(page, pageSize);
            return Ok(result);
        }

        [Authorize(Roles = Constants.Roles.Admin)]
        [HttpPatch("{orderId}/Status")]
        public async Task<ActionResult> SetOrderStatus(int orderId, [FromQuery] UpdateOrderStatusDto dto)
        {
            await _orderService.SetOrderStatus(orderId, dto.Status);
            return NoContent();
        }
    }
}