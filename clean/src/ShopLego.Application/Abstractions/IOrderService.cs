namespace ShopLego.Application;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken ct);
    Task<IReadOnlyList<OrderDto>> GetByUserAsync(int userId, CancellationToken ct);
    Task<OrderDto?> GetAsync(int id, CancellationToken ct);
    Task<int> PlaceAsync(int userId, string address, CancellationToken ct);
    Task<int> BuyNowAsync(int userId, int productId, int quantity, string address, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int id, int statusId, CancellationToken ct);
}
