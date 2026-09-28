namespace ShopLego.Application;

public interface IReferenceDataService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct);
    Task<IReadOnlyList<OrderStatusDto>> GetOrderStatusesAsync(CancellationToken ct);
    Task<IReadOnlyList<EmailLogDto>> GetEmailLogsAsync(CancellationToken ct);
    Task<OrderDetailDto?> GetOrderDetailAsync(int id, CancellationToken ct);
    Task<CartDto?> GetCartAsync(int userId, CancellationToken ct);
}
