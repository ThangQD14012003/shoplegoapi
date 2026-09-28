using Microsoft.EntityFrameworkCore;
using ShopLego.Application;
using ShopLego.Domain.Entities;
using ShopLego.Infrastructure.Persistence;
namespace ShopLego.Infrastructure.Services;

public sealed class CartService(ShopLegoDbContext db) : ICartService
{
    public async Task<IReadOnlyList<CartItemDto>> GetAsync(int userId, CancellationToken ct) => await db.CartItems.AsNoTracking().Where(x => x.Cart.UserId == userId).Select(x => new CartItemDto(x.Id, x.ProductId, x.Quantity, x.Product.Name, x.Product.Price, x.Product.ImageUrl)).ToListAsync(ct);
    public async Task AddAsync(int userId, int productId, CancellationToken ct) { var p = await db.Products.FindAsync([productId], ct) ?? throw new NotFoundException("Product not found."); if (p.AvailableQuantity < 1) throw new BusinessRuleException("Product is out of stock."); var c = await db.Carts.Include(x => x.CartItems).SingleOrDefaultAsync(x => x.UserId == userId, ct); if (c is null) { c = new Cart { UserId = userId, CreatedAt = DateTime.UtcNow }; db.Carts.Add(c); } var i = c.CartItems.SingleOrDefault(x => x.ProductId == productId); if (i is null) c.CartItems.Add(new CartItem { ProductId = productId, Quantity = 1, UnitPrice = p.Price }); else if (i.Quantity >= p.AvailableQuantity) throw new BusinessRuleException("Requested quantity exceeds available stock."); else i.Quantity++; await db.SaveChangesAsync(ct); }
    public async Task<bool> RemoveAsync(int userId, int itemId, CancellationToken ct) { var i = await db.CartItems.SingleOrDefaultAsync(x => x.Id == itemId && x.Cart.UserId == userId, ct); if (i is null) return false; db.CartItems.Remove(i); await db.SaveChangesAsync(ct); return true; }
    public async Task ClearAsync(int userId, CancellationToken ct) { var items = await db.CartItems.Where(x => x.Cart.UserId == userId).ToListAsync(ct); db.CartItems.RemoveRange(items); await db.SaveChangesAsync(ct); }
    public async Task<bool> SetQuantityAsync(int userId, int itemId, int quantity, CancellationToken ct) { if (quantity < 1) throw new BusinessRuleException("Quantity must be at least 1."); var i = await db.CartItems.Include(x => x.Product).SingleOrDefaultAsync(x => x.Id == itemId && x.Cart.UserId == userId, ct); if (i is null) return false; if (quantity > i.Product.AvailableQuantity) throw new BusinessRuleException("Requested quantity exceeds available stock."); i.Quantity = quantity; await db.SaveChangesAsync(ct); return true; }
}
