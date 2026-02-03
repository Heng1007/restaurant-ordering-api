using System.Text.Json;

namespace FoodDeliveryServer.Dtos
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;

        // 👇 一个方便的小方法：把自己变成 JSON 字符串
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
