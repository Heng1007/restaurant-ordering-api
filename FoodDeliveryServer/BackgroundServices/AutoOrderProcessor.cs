using FoodDeliveryServer.Data;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryServer.BackgroundServices
{
    public class AutoOrderProcessor: BackgroundService
    {
        private readonly ILogger<AutoOrderProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public AutoOrderProcessor(ILogger<AutoOrderProcessor> logger, IServiceScopeFactory serviceFactory)
        {
            _logger = logger;
            _scopeFactory = serviceFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Auto Background Processor is starting.");

            while (!stoppingToken.IsCancellationRequested){
                try
                {
                    await DoWorkAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing Auto Background Processor.");
                }

                _logger.LogInformation("Auto Background Processor is delaying for 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task DoWorkAsync()
        {
            // 👇 重点！手动创建一个 Scope (范围)
            // 就像手动模拟一次 "HTTP 请求" 的生命周期
            using (var scope = _scopeFactory.CreateScope())
            {
                // 从 Scope 里拿出数据库连接 (AppDbContext)
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 简单的任务：数一下有多少个订单
                var count = await context.Orders.CountAsync();

                _logger.LogInformation($" Autocount: {count} orders");

                // --- 未来你想写的复杂逻辑都在这里写 ---
                // 比如: var expiredOrders = context.Orders.Where(...)
                // context.RemoveRange(expiredOrders);
                // await context.SaveChangesAsync();
            }
            // 👈 出了这里，Scope 销毁，数据库连接自动断开 (一次性抹布扔掉)
        }
    }
}
