using FoodDeliveryServer.Data;
using FoodDeliveryServer.Models;
using FoodDeliveryServer.Services;
using Microsoft.EntityFrameworkCore;
using Moq; // 👈 模拟工具
using Xunit; // 👈 测试框架

namespace FoodDeliveryServer.Tests
{
    public class OrderServiceTests
    {
        // 这是一个辅助方法，用来创建一个“假数据库”
        // 每次测试前，我们都想要一个干干净净的内存数据库
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 给每个测试一个唯一的名字，防止冲突
                .Options;
            return new AppDbContext(options);
        }

        [Fact] // 👈 告诉系统这是一个测试
        public async Task CancelOrder_ShouldSucceed_WhenOrderIsPendingAndUserIsOwner()
        {
            // —————— 1. Arrange (准备阶段：布置场景) ——————

            // A. 准备假数据库
            var context = GetInMemoryDbContext();

            // B. 往假数据库里塞一个“模拟订单”
            var orderId = 1;
            var userId = 100;
            context.Orders.Add(new Order
            {
                Id = orderId,
                UserId = userId,       // 这是号主
                Status = OrderStatus.Pending, // 状态是可以取消的 Pending
                TotalPrice = 50
            });
            await context.SaveChangesAsync(); // 保存假数据

            // C. 准备 AI 服务的替身 (因为 OrderService 需要 IAIService)
            var mockAiService = new Mock<IAIService>();

            // D. 创建我们要测试的主角：OrderService
            var service = new OrderService(context, mockAiService.Object);

            // —————— 2. Act (行动阶段：调用方法) ——————
            // 尝试取消订单
            var result = await service.CancelOrder(orderId, userId);

            // —————— 3. Assert (验证阶段：检查结果对不对) ——————

            // 验证 A: 返回值应该是 null (代表成功，没有错误信息)
            Assert.Null(result);

            // 验证 B: 再去查数据库，确认状态真的变成了 Cancelled
            var updatedOrder = await context.Orders.FindAsync(orderId);
            Assert.Equal(OrderStatus.Cancelled, updatedOrder!.Status);
        }

        [Fact]
        public async Task CancelOrder_ShouldFail_WhenUserIsNotOwner()
        {
            // —————— Arrange ——————
            var context = GetInMemoryDbContext();

            // 我们只需要定义订单的主人是 1 号
            int ownerId = 1;
            int hackerId = 2; // 黑客 ID

            context.Orders.Add(new Order
            {
                Id = 999,
                UserId = ownerId, // 👈 归属权：1号
                Status = OrderStatus.Pending,
                TotalPrice = 100
            });
            await context.SaveChangesAsync();

            var mockIAiService = new Mock<IAIService>();
            var orderService = new OrderService(context, mockIAiService.Object);

            // —————— Action ——————
            // ❌ 删掉 User 1 的操作，不要让他碰这个订单！保持订单是 Pending 状态。

            // 只让黑客去试
            var result = await orderService.CancelOrder(999, hackerId);

            // —————— Assert ——————
            // 1. 验证必须报错 (不能是 null)
            Assert.NotNull(result);

            // 2. 验证报错信息必须精准
            // ⚠️ 警告：去你的 OrderService.cs 复制粘贴那句报错，一个标点符号都不能错！
            // 假设你的代码里写的是 "你无权操作他人的订单"
            Assert.Equal("You do not have permission to cancel this order.", result);
        }

        [Fact]
        public async Task CancelOrder_ShouldFail_WhenOrderIsAlreadyDelivered()
        {
            var context = GetInMemoryDbContext();
            var ownerId = 1;
            var deliveredOrder = new Order
            {
                Id = 999,
                UserId = ownerId,
                Status = OrderStatus.Delivered, // 已经送达
                TotalPrice = 100
            };

            var mockAIService = new Mock<IAIService>();
            var orderService = new OrderService(context, mockAIService.Object);

            context.Orders.Add(deliveredOrder); //添加订单

            await context.SaveChangesAsync();

            var CancelResult = await orderService.CancelOrder(999, ownerId);

            Assert.NotNull(CancelResult);
            Assert.Equal("Only pending orders can be cancelled.", CancelResult);
        }
    }
}