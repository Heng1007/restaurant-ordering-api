using Xunit;
using Microsoft.EntityFrameworkCore;
using FoodDeliveryServer.Data;
using FoodDeliveryServer.Services;
using FoodDeliveryServer.Models;
using System.Threading.Tasks;
using System;

namespace FoodDeliveryServer.Tests
{
    public class FoodServiceTests
    {
        // 🛠 1. 准备工具：造一个“全息投影”的假数据库 (InMemory)
        // 每次运行都会生成一个新的数据库名，保证测试之间互不干扰
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact] // 👈 这个标签告诉系统：这是一个测试用例
        public async Task AddFood_Should_Save_Food_To_Database()
        {
            // 🔶 A1: Arrange (准备场景)
            var context = GetInMemoryDbContext(); // 拿到假数据库
            var service = new FoodService(context); // 把假数据库塞给厨师(Service)

            var newFood = new FoodItem
            {
                Name = "Test Nasi Lemak",
                Price = 12.50m
            };

            // 🔶 A2: Act (执行动作)
            // 让厨师真的去加菜
            await service.AddFood(newFood);

            // 🔶 A3: Assert (验证结果)
            // 机器人拿着放大镜去数据库里检查：

            // 检查1：数据库里的数量是不是变成了 1？
            var count = await context.FoodItems.CountAsync();
            Assert.Equal(1, count);

            // 检查2：那道菜的名字对不对？
            var savedFood = await context.FoodItems.FirstAsync();
            Assert.Equal("Test Nasi Lemak", savedFood.Name);
        }
    }
}