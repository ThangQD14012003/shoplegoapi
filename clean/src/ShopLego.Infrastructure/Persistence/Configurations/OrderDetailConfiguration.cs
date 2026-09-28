using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail> { public void Configure(EntityTypeBuilder<OrderDetail> b) { b.ToTable("OrderDetails"); b.HasKey(x => x.Id); b.HasOne(x => x.Order).WithMany(x => x.OrderDetails).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade); b.HasOne(x => x.Product).WithMany(x => x.OrderDetails).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict); } }
