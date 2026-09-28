namespace ShopLego.Application;

public interface IIdentityService
{
    Task<int> RegisterAsync(RegisterRequest request, CancellationToken ct);
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct);
    Task<AuthResponse?> RefreshAsync(string refreshToken, CancellationToken ct);
}
