using FoodDeliveryServer.Dtos;
using System.Net;

namespace FoodDeliveryServer.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next; // 下一棒接力者
        private readonly ILogger<ExceptionMiddleware> _logger; // 记录日志

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // 👇 核心逻辑：拦截所有请求
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 1. 尝试放行，让请求去 Controller
                await _next(context);
            }
            catch (Exception ex)
            {
                // 2. 如果 Controller 报错了，这里会捉住它！
                _logger.LogError(ex, $"Something went wrong: {ex.Message}"); // 记下来给开发者看

                // 3. 处理错误，返回优雅的 JSON 给用户看
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // 设置 HTTP 状态码为 500 (服务器内部错误)
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 创建我们刚才定义的标准错误模型
            var response = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                // 为了安全，生产环境通常只返回 "Internal Server Error"，不给看具体错误
                // 但为了调试方便，我们先返回 ex.Message
                Message = "服务器开小差了: " + exception.Message
            };

            // 写入响应
            await context.Response.WriteAsync(response.ToString());
        }
    }
}
