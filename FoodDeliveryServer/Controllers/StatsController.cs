using FoodDeliveryServer.Data;
using FoodDeliveryServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StatsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Stats/BestSellers
        // 获取过去 7 天最受欢迎的 3 道菜
        [HttpGet("BestSellers")]
        public async Task<ActionResult> GetBestSellers()
        {
            var stats = await _context.Orders
                // 1. 筛选：只要最近 7 天的订单
                .Where(o => o.OrderDate >= DateTime.Now.AddDays(-7))

                // 2. 关联：必须把 FoodItem 拉进来，不然不知道菜名
                .Include(o => o.FoodItem)

                // 3. 分组：把相同名字的菜堆在一起 (比如所有的 Burger 放一堆)
                .GroupBy(o => o.FoodItem.Name)

                // 4. 统计：对于每一堆(g)，我们要算出什么？
                .Select(g => new
                {
                    Name = g.Key,                       // 菜名
                    TotalSold = g.Sum(o => o.Quantity)  // 卖出的总数量 (把这一堆的 Quantity 加起来)
                })

                // 5. 排序：卖得最多的排在最上面 (Descending = 降序)
                .OrderByDescending(x => x.TotalSold)

                // 6. 截取：只取前 3 名
                .Take(3)

                // 7. 执行查询
                .ToListAsync();

            return Ok(stats);
        }

        [HttpGet("TopSpender")]
        public async Task<ActionResult> GetTopSpender()
        {
            var stats = await _context.Orders
                .Include(o => o.FoodItem)
                .GroupBy(o => o.CustomerName)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    TotalSpent = g.Sum(o => o.Quantity * o.FoodItem.Price)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(1)
                .ToListAsync();
            return Ok(stats);
        }
    }
}