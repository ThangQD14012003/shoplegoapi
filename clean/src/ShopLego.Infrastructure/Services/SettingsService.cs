using Microsoft.EntityFrameworkCore;
using ShopLego.Application;
using ShopLego.Domain.Entities;
using ShopLego.Infrastructure.Persistence;
namespace ShopLego.Infrastructure.Services;

public sealed class SettingsService(ShopLegoDbContext db) : ISettingsService
{
    public async Task<SettingsDto> GetAsync(CancellationToken ct) { var v = await db.SystemSettings.AsNoTracking().ToDictionaryAsync(x => x.Key, x => x.Value, ct); return new(v.GetValueOrDefault("ManagerEmail", "manager@example.com"), v.GetValueOrDefault("AccountantEmail", "accountant@example.com")); }
    public async Task UpdateAsync(SettingsDto s, CancellationToken ct) { await Upsert("ManagerEmail", s.ManagerEmail, ct); await Upsert("AccountantEmail", s.AccountantEmail, ct); await db.SaveChangesAsync(ct); }
    private async Task Upsert(string key, string value, CancellationToken ct) { var i = await db.SystemSettings.FindAsync([key], ct); if (i is null) db.SystemSettings.Add(new SystemSetting { Key = key, Value = value.Trim() }); else i.Value = value.Trim(); }
}
