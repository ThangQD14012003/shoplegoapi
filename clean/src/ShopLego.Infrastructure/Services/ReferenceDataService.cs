using Microsoft.EntityFrameworkCore;
using ShopLego.Application;
using ShopLego.Infrastructure.Persistence;
namespace ShopLego.Infrastructure.Services;

public sealed class ReferenceDataService(ShopLegoDbContext db) : IReferenceDataService
{
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct) => await db.Categories.AsNoTracking().OrderBy(x => x.Name).Select(x => new CategoryDto(x.Id, x.Name, x.Description)).ToListAsync(ct);
    public async Task<IReadOnlyList<OrderStatusDto>> GetOrderStatusesAsync(CancellationToken ct) => await db.OrderStatuses.AsNoTracking().OrderBy(x => x.Id).Select(x => new OrderStatusDto(x.Id, x.Name)).ToListAsync(ct);
    public async Task<IReadOnlyList<EmailLogDto>> GetEmailLogsAsync(CancellationToken ct) => await db.EmailLogs.AsNoTracking().OrderByDescending(x => x.SendTime).Select(x => new EmailLogDto(x.Id, x.OrderId, x.ReceiverEmail, x.Subject, x.SendTime, x.Status)).ToListAsync(ct);
    public async Task<OrderDetailDto?> GetOrderDetailAsync(int id, CancellationToken ct) => await db.OrderDetails.AsNoTracking().Where(x => x.Id == id).Select(x => new OrderDetailDto(x.Id, x.ProductId, x.Product.Name, x.Product.ImageUrl, x.Quantity, x.UnitPrice)).SingleOrDefaultAsync(ct);
    public async Task<CartDto?> GetCartAsync(int userId, CancellationToken ct) => await db.Carts.AsNoTracking().Where(x => x.UserId == userId).Select(x => new CartDto(x.Id, x.UserId, x.CreatedAt, x.CartItems.Select(i => new CartItemDto(i.Id, i.ProductId, i.Quantity, i.Product.Name, i.Product.Price, i.Product.ImageUrl)).ToList())).SingleOrDefaultAsync(ct);
}
