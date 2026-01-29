using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;

namespace FoodDeliveryServer.Services
{
    public class AIService : IAIService
    {
        private readonly string _endpoint;
        private readonly string _key;
        private readonly TextAnalyticsClient _client;

        public AIService(IConfiguration configuration)
        {
            _endpoint = configuration["AzureSettings:Endpoint"]
                ?? throw new InvalidOperationException("Azure Endpoint is missing!");

            _key = configuration["AzureSettings:Key"]
                   ?? throw new InvalidOperationException("Azure Key is missing!");

            // 初始化客户端 (拨通电话)
            var credentials = new AzureKeyCredential(_key);
            var endpointUri = new Uri(_endpoint);
            _client = new TextAnalyticsClient(endpointUri, credentials);
        }

        public async Task<string> AnalyzeSentiment(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "Neutral";

            try
            {
                // 1. 调用 Azure AI (这就是在那一瞬间发生的云端交互)
                DocumentSentiment result = await _client.AnalyzeSentimentAsync(text);

                // 2. 获取结果 (Positive, Negative, Neutral, Mixed)
                return result.Sentiment.ToString();
            }
            catch (Exception ex)
            {
                // 如果断网了或者 Key 错了，为了不让程序崩掉，我们返回一个默认值
                Console.WriteLine($"AI Error: {ex.Message}");
                return "Error";
            }
        }
    }
}