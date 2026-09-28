using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category> { public void Configure(EntityTypeBuilder<Category> b) { b.ToTable("Categories"); b.HasKey(x => x.Id); } }
