using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting> { public void Configure(EntityTypeBuilder<SystemSetting> b) { b.ToTable("SystemSettings"); b.HasKey(x => x.Key); } }
