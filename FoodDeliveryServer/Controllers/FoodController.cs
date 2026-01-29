using FoodDeliveryServer.Models;
using FoodDeliveryServer.Services; // 👈 记得引用这个！
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : ControllerBase
    {
        // 1. 以前这里是 _context，现在变成了 _foodService
        private readonly IFoodService _foodService;

        // 2. 构造函数：向系统要一个“懂 IFoodService 标准”的人
        public FoodController(IFoodService foodService)
        {
            _foodService = foodService;
        }

        // GET: api/Food
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItems()
        {
            // 3. 不再自己查数据库，而是直接指挥 Service
            var foods = await _foodService.GetAllFoods();
            return Ok(foods);
        }

        // POST: api/Food
        [HttpPost]
        public async Task<ActionResult<FoodItem>> PostFoodItem(FoodItem foodItem)
        {
            // 指挥 Service 添加
            await _foodService.AddFood(foodItem);
            
            return CreatedAtAction(nameof(GetFoodItems), new { id = foodItem.Id }, foodItem);
        }
    }
}