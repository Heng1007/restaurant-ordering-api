using FoodDeliveryServer.Data;
using FoodDeliveryServer.Dtos;
using FoodDeliveryServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FoodDeliveryServer.Tests
{
    public class AuthServiceTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // 给每个测试一个唯一的名字，防止冲突
                .Options;
            return new AppDbContext(options);
        }


        [Fact]
        public async Task Register_ShouldFail_WhenUsernameAlreadyExists()
        {
            // —————— Arrange (准备) ——————
            var context = GetInMemoryDbContext();

            // 1. 准备用户数据
            var userDto = new UserDto
            {
                Username = "Heng",
                Password = "123456"
            };

            // 2. 准备 Configuration (配角)
            // 因为 AuthService 内部生成 Token 时需要读取 "Jwt:Key"
            var mockConfig = new Mock<IConfiguration>();
            // 告诉假 Config：如果有人问你要 "Jwt:Key"，你就给他 "super_secret_key_123456789"
            mockConfig.Setup(c => c["MyJwtKey"]).Returns("super_secret_key_123456789_must_be_long_enough");

            // 3. 准备假 Logger (参数 3) 👈 新加的！
            // 这里的 <AuthService> 是告诉 Moq：我要模拟的是 AuthService 专用的 Logger
            var mockLogger = new Mock<ILogger<AuthService>>();

            // 3. 创建主角 AuthService
            // 注意：这里不需要 Mock<IAuthService>，因为我们就是在测它！
            // 假设你的构造函数是 (DbContext, IConfiguration)
            var service = new AuthService(context, mockConfig.Object, mockLogger.Object);

            // —————— Act (行动) ——————

            // 动作 1: 第一次注册 (应该成功)
            var successResult = await service.Register(userDto);

            // 动作 2: 用同样的名字再注册一次 (应该失败)
            var failResult = await service.Register(userDto);

            // —————— Assert (验证) ——————

            // 验证 1: 第一次应该成功 (返回 Token，不为空)
            Assert.NotNull(successResult);
            // 这里的判断逻辑取决于你的 AuthService 具体怎么写
            // 如果成功返回 Token (长字符串)，失败返回错误信息 (短字符串)
            Assert.True(successResult.Length > 50); // Token 通常很长

            // 验证 2: 第二次应该失败 (返回错误信息)
            // 🚨 重点：我们要检查 failResult，而不是 successResult
            Assert.Equal("Username already exists.", failResult);
        }

    }
}
