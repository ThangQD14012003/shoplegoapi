using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User> { public void Configure(EntityTypeBuilder<User> b) { b.ToTable("Users"); b.HasKey(x => x.Id); } }
