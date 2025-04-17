using Microsoft.EntityFrameworkCore;
using net9购物网站.MODEL;

namespace net9购物网站.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置 Price 属性的精度和小数位数
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2); // 精度为 18，总共 18 位，其中小数位为 2

            // Additional configurations if needed
        }
    }
}
