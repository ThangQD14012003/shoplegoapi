using Microsoft.EntityFrameworkCore;
using ShopLego.Application;
using ShopLego.Domain.Entities;
using ShopLego.Infrastructure.Persistence;
namespace ShopLego.Infrastructure.Services;

public sealed class ProductService(ShopLegoDbContext db) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct) => await db.Products.AsNoTracking().OrderBy(x => x.Name).Select(x => Map(x)).ToListAsync(ct);
    public async Task<ProductDto?> GetAsync(int id, CancellationToken ct) => await db.Products.AsNoTracking().Where(x => x.Id == id).Select(x => Map(x)).SingleOrDefaultAsync(ct);
    public async Task<int> CreateAsync(ProductRequest r, CancellationToken ct) { await EnsureCategory(r.CategoryId, ct); var now = DateTime.UtcNow; var e = new Product { CategoryId = r.CategoryId, Name = r.Name.Trim(), Description = r.Description.Trim(), Price = r.Price, StockQuantity = r.StockQuantity, AvailableQuantity = r.AvailableQuantity, ImageUrl = r.ImageUrl.Trim(), CreatedAt = now, UpdatedAt = now }; db.Products.Add(e); await db.SaveChangesAsync(ct); return e.Id; }
    public async Task<bool> UpdateAsync(int id, ProductRequest r, CancellationToken ct) { var e = await db.Products.FindAsync([id], ct); if (e is null) return false; await EnsureCategory(r.CategoryId, ct); e.CategoryId = r.CategoryId; e.Name = r.Name.Trim(); e.Description = r.Description.Trim(); e.Price = r.Price; e.StockQuantity = r.StockQuantity; e.AvailableQuantity = r.AvailableQuantity; e.ImageUrl = r.ImageUrl.Trim(); e.UpdatedAt = DateTime.UtcNow; await db.SaveChangesAsync(ct); return true; }
    public async Task<bool> DeleteAsync(int id, CancellationToken ct) { var e = await db.Products.FindAsync([id], ct); if (e is null) return false; db.Products.Remove(e); await db.SaveChangesAsync(ct); return true; }
    private async Task EnsureCategory(int id, CancellationToken ct) { if (!await db.Categories.AnyAsync(x => x.Id == id, ct)) throw new BusinessRuleException("Category does not exist."); }
    private static ProductDto Map(Product x) => new(x.Id, x.CategoryId, x.Name, x.Description, x.Price, x.StockQuantity, x.AvailableQuantity, x.ImageUrl, x.CreatedAt, x.UpdatedAt);
}
