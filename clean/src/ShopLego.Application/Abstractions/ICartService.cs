namespace ShopLego.Application;

public interface ICartService
{
    Task<IReadOnlyList<CartItemDto>> GetAsync(int userId, CancellationToken ct);
    Task AddAsync(int userId, int productId, CancellationToken ct);
    Task<bool> RemoveAsync(int userId, int itemId, CancellationToken ct);
    Task ClearAsync(int userId, CancellationToken ct);
    Task<bool> SetQuantityAsync(int userId, int itemId, int quantity, CancellationToken ct);
}
