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
            context.Response.ContentType = "application/json";

            // 👇 这里的 switch 是核心！根据异常类型决定状态码
            switch (exception)
            {
                // 如果是“未授权”异常 (密码错) -> 返回 401 Unauthorized
                case UnauthorizedAccessException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    break;

                // 如果是“参数不对”异常 (比如库存不足) -> 返回 400 Bad Request
                // 你以后可以用 throw new ArgumentException("库存没了");
                case ArgumentException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;

                // 其他所有未知的错误 -> 返回 500 Internal Server Error
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            // 构建错误响应
            var response = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                // 如果是 500，为了安全不要把 exception.Message 给用户看，可以写 "Internal Server Error"
                // 如果是 401/400，把 Message 给用户看 ("Username or password incorrect.")
                Message = context.Response.StatusCode == 500 ? "Internal Server Error" : exception.Message
            };

            await context.Response.WriteAsync(response.ToString());
        }
    }
}
