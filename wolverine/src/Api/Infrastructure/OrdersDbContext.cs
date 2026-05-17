using Microsoft.EntityFrameworkCore;
using WolverinePoc.Api.Features.Orders;

namespace WolverinePoc.Api.Infrastructure;

public sealed class OrdersDbContext(DbContextOptions<OrdersDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(order => order.Id);
            entity.Property(order => order.Id).HasColumnName("id");
            entity.Property(order => order.CustomerName).HasColumnName("customer_name").HasColumnType("text").IsRequired();
            entity.Property(order => order.ProductCode).HasColumnName("product_code").HasColumnType("text").IsRequired();
            entity.Property(order => order.Quantity).HasColumnName("quantity").IsRequired();
            entity.Property(order => order.UnitPrice).HasColumnName("unit_price").HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(order => order.TotalAmount).HasColumnName("total_amount").HasColumnType("numeric(18,2)").IsRequired();
            entity.Property(order => order.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();
            entity.Property(order => order.CreatedUtc)
                .HasColumnName("created_utc")
                .HasColumnType("timestamp with time zone")
                .IsRequired();
        });
    }
}
