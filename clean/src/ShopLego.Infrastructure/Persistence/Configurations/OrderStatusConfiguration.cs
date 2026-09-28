using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus> { public void Configure(EntityTypeBuilder<OrderStatus> b) { b.ToTable("OrderStatuses"); b.HasKey(x => x.Id); } }
