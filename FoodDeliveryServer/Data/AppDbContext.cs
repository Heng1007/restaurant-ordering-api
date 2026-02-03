using FoodDeliveryServer.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodDeliveryServer.Data
{
    // 继承 DbContext，这就变成了 EF Core 的数据库管家
    public class AppDbContext : DbContext
    {
        // 构造函数：接收配置（比如数据库连接字符串）传给父类
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 这就是你的“表”。
        // DbSet<FoodItem> 表示数据库里会有一张表叫 "FoodItems"
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<string>(); // 👈 这行代码是救命稻草！
                                          // 它的作用：
                                          // 存的时候：把 Enum.Pending 变成 "Pending"
                                          // 取的时候：把 "Pending" 变成 Enum.Pending
        }
    }
}