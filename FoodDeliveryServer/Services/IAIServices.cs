namespace FoodDeliveryServer.Services
{
    public interface IAIService
    {
        // 输入一句话，返回情感结果 (比如 "Positive", "Negative", "Neutral")
        Task<string> AnalyzeSentiment(string text);
    }
}
