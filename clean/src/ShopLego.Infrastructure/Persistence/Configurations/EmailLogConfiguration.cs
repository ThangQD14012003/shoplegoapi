using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Persistence.Configurations;

public sealed class EmailLogConfiguration : IEntityTypeConfiguration<EmailLog> { public void Configure(EntityTypeBuilder<EmailLog> b) { b.ToTable("EmailLogs"); b.HasKey(x => x.Id); b.HasOne(x => x.Order).WithMany(x => x.EmailLogs).HasForeignKey(x => x.OrderId).OnDelete(DeleteBehavior.Cascade); } }
