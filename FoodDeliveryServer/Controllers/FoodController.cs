using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Models;
using FoodDeliveryServer.Services; // 👈 记得引用这个！
using FoodDeliveryServer.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [Authorize(Roles = Constants.Roles.Admin)]
        // POST: api/Food
        [HttpPost]
        public async Task<ActionResult<FoodItem>> PostFoodItem([FromForm] FoodCreateDto request)
        {
            var foodItem = new FoodItem
            {
                Name = request.Name,
                Price = request.Price
            };
            
            if(request.Image != null)
            {
                // a. 生成一个唯一的文件名 (比如: pizza_GUID.jpg)，防止名字重复覆盖
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(request.Image.FileName);
                // b. 拼凑出保存到硬盘的绝对路径 (你的电脑/wwwroot/images/xxx.jpg)
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                // c. 创建文件流，把图片存进去
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.Image.CopyToAsync(stream);
                }

                // d. 把“网络路径”存到数据库 (注意：是存 URL，不是存文件本身)
                // 比如: /images/xxx.jpg
                foodItem.ImageUrl = "/images/" + fileName;
            }

            var createdFoodItem = await _foodService.AddFood(foodItem);

            return Ok(createdFoodItem);
        }

        [Authorize(Roles = Constants.Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFoodItem(int id)
        {
            await _foodService.DeleteFood(id);
            return NoContent();
        }


        [HttpGet("error-test")] // 访问地址: GET /api/Food/error-test
        public IActionResult GenerateError()
        {
            // 故意抛出一个异常
            throw new Exception("这是我故意制造的爆炸！💥");
        }
    }
}