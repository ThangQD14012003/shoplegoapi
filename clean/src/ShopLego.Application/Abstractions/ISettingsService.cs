namespace ShopLego.Application;

public interface ISettingsService
{
    Task<SettingsDto> GetAsync(CancellationToken ct);
    Task UpdateAsync(SettingsDto settings, CancellationToken ct);
}
