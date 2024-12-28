using EasyKart.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace EasyKart.Orders.Repositories
{
    public class OrdersDBContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public OrdersDBContext(DbContextOptions<OrdersDBContext> options) : base(options)
        {
        }

        override protected void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.UserId)
                      .IsRequired();

                entity.Property(o => o.Price)
                      .IsRequired();

                entity.Property(o => o.Status)
                      .HasMaxLength(50); // Optional: Limit the status string length

                // Configure one-to-many relationship with OrderItem
                entity.HasMany(o => o.Items)
                      .WithOne(oi => oi.Order)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // Optional: Cascade delete
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.Id);

                entity.Property(oi => oi.OrderId)
                      .IsRequired();

                entity.Property(oi => oi.ProductId)
                      .IsRequired();

                entity.Property(oi => oi.Quantity)
                      .IsRequired();

                // Define foreign key to Order
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(oi => oi.OrderId);
            });
        }
    }
}
