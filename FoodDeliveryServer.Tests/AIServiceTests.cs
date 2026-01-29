using FoodDeliveryServer.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using Xunit;

namespace FoodDeliveryServer.Tests
{
    public class AIServiceTests
    {
        private IConfiguration GetMockConfiguration()
        {
            // ❌ 旧写法：直接写死 (删掉!)
            // var mySettings = new Dictionary<string, string> ... 

            // ✅ 新写法：告诉它去读“用户机密保险箱”
            // 注意：这里的 <AIServiceTests> 是为了定位保险箱 ID
            return new ConfigurationBuilder()
                .AddUserSecrets<AIServiceTests>()
                .Build();
        }

        [Fact]
        public async Task AnalyzeSentiment_Should_Return_Positive_For_Good_Text()
        {
            // 1. Arrange (准备)
            // 这里我们直接 new 一个 Service，因为它内部已经写死 Key 了
            var config = GetMockConfiguration();
            var service = new AIService(config);
            var input = "The food is amazing! I love it!"; // 测试语句：我想吃好的！

            // 2. Act (行动 - 真的去连 Azure 了！)
            var result = await service.AnalyzeSentiment(input);

            // 3. Assert (验证)
            // Azure 应该觉得这句话是 Positive (正面) 的
            Assert.Equal("Positive", result);
        }

        [Fact]
        public async Task AnalyzeSentiment_Should_Return_Negative_For_Bad_Text()
        {
            var config = GetMockConfiguration();
            var service = new AIService(config);
            var input = "The service is terrible and the food is cold."; // 测试语句：差评！

            var result = await service.AnalyzeSentiment(input);

            Assert.Equal("Negative", result);
        }
    }
}