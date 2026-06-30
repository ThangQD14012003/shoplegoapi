using Microsoft.EntityFrameworkCore;

namespace ShopLegoApi.Datas
{
    public class LegoDataContext : DbContext
    {
        public LegoDataContext(DbContextOptions<LegoDataContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<OrderStatus> OrderStatuses { get; set; }

        public DbSet<EmailLog> EmailLogs { get; set; }

        public DbSet<SystemSetting> SystemSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // Category - Product
            // =========================

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // User - Cart
            // =========================

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany(u => u.Carts)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Cart - CartItem
            // =========================

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Product - CartItem
            // =========================

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // User - Order
            // =========================

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // OrderStatus - Order
            // =========================

            modelBuilder.Entity<Order>()
                .HasOne(o => o.OrderStatus)
                .WithMany(os => os.Orders)
                .HasForeignKey(o => o.OrderStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Order - OrderDetail
            // =========================

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Product - OrderDetail
            // =========================

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // =========================
            // Order - EmailLog
            // =========================

            modelBuilder.Entity<EmailLog>()
                .HasOne(el => el.Order)
                .WithMany()
                .HasForeignKey(el => el.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // Seed Order Status
            // =========================

            modelBuilder.Entity<OrderStatus>().HasData(
                new OrderStatus { Id = 1, Name = "Pending" },
                new OrderStatus { Id = 2, Name = "Confirmed" },
                new OrderStatus { Id = 3, Name = "Shipping" },
                new OrderStatus { Id = 4, Name = "Completed" },
                new OrderStatus { Id = 5, Name = "Cancelled" }
            );

            // =========================
            // Seed System Settings
            // =========================
            modelBuilder.Entity<SystemSetting>().HasData(
                new SystemSetting { Key = "ManagerEmail", Value = "manager@example.com" },
                new SystemSetting { Key = "AccountantEmail", Value = "accountant@example.com" }
            );
        }
    }
}