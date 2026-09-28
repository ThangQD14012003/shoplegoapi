using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class CartItemConfiguration : IEntityTypeConfiguration<CartItem> { public void Configure(EntityTypeBuilder<CartItem> b) { b.ToTable("CartItems"); b.HasKey(x => x.Id); b.HasOne(x => x.Cart).WithMany(x => x.CartItems).HasForeignKey(x => x.CartId).OnDelete(DeleteBehavior.Cascade); b.HasOne(x => x.Product).WithMany(x => x.CartItems).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Restrict); } }
