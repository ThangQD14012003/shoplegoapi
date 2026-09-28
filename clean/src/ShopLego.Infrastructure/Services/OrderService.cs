using Microsoft.EntityFrameworkCore;
using ShopLego.Application;
using ShopLego.Domain.Constants;
using ShopLego.Domain.Entities;
using ShopLego.Infrastructure.Persistence;

namespace ShopLego.Infrastructure.Services;

public sealed class OrderService(ShopLegoDbContext db, IOrderNotifier notifier) : IOrderService
{
    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken ct) =>
        await Query().OrderByDescending(x => x.OrderDate).Select(x => Map(x)).ToListAsync(ct);

    public async Task<IReadOnlyList<OrderDto>> GetByUserAsync(int userId, CancellationToken ct) =>
        await Query().Where(x => x.UserId == userId).OrderByDescending(x => x.OrderDate).Select(x => Map(x)).ToListAsync(ct);

    public async Task<OrderDto?> GetAsync(int id, CancellationToken ct) =>
        await Query().Where(x => x.Id == id).Select(x => Map(x)).SingleOrDefaultAsync(ct);

    public async Task<int> PlaceAsync(int userId, string address, CancellationToken ct)
    {
        var cart = await db.Carts.Include(x => x.CartItems).ThenInclude(x => x.Product)
            .SingleOrDefaultAsync(x => x.UserId == userId, ct);
        if (cart is null || cart.CartItems.Count == 0) throw new BusinessRuleException("Cart is empty.");
        return await CreateOrder(userId, address, cart.CartItems.Select(x => (x.Product, x.Quantity)).ToList(), cart.CartItems, ct);
    }

    public async Task<int> BuyNowAsync(int userId, int productId, int quantity, string address, CancellationToken ct)
    {
        if (quantity < 1) throw new BusinessRuleException("Quantity must be at least 1.");
        var product = await db.Products.FindAsync([productId], ct) ?? throw new NotFoundException("Product not found.");
        return await CreateOrder(userId, address, [(product, quantity)], null, ct);
    }

    public async Task<bool> UpdateStatusAsync(int id, int statusId, CancellationToken ct)
    {
        if (statusId is < 1 or > 5) throw new BusinessRuleException("Invalid order status.");
        var order = await db.Orders.Include(x => x.OrderDetails).ThenInclude(x => x.Product).SingleOrDefaultAsync(x => x.Id == id, ct);
        if (order is null) return false;
        if (order.OrderStatusId == OrderStatusIds.Cancelled && statusId != OrderStatusIds.Cancelled)
            throw new BusinessRuleException("A cancelled order cannot be reopened.");
        if (statusId == OrderStatusIds.Completed && order.OrderStatusId != OrderStatusIds.Completed)
            foreach (var detail in order.OrderDetails) detail.Product.StockQuantity -= detail.Quantity;
        if (statusId == OrderStatusIds.Cancelled && order.OrderStatusId != OrderStatusIds.Cancelled)
            foreach (var detail in order.OrderDetails) detail.Product.AvailableQuantity += detail.Quantity;
        order.OrderStatusId = statusId;
        await db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<int> CreateOrder(int userId, string address, IReadOnlyList<(Product Product, int Quantity)> lines,
        IEnumerable<CartItem>? cartItems, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(address)) throw new BusinessRuleException("Shipping address is required.");
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var order = new Order
        {
            UserId = userId,
            OrderStatusId = OrderStatusIds.Pending,
            OrderDate = DateTime.UtcNow,
            ShippingAddress = address.Trim(),
            TotalAmount = lines.Sum(x => x.Product.Price * x.Quantity)
        };
        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        foreach (var line in lines)
        {
            var affected = await db.Database.ExecuteSqlInterpolatedAsync(
                $"UPDATE \"Products\" SET \"AvailableQuantity\" = \"AvailableQuantity\" - {line.Quantity} WHERE \"Id\" = {line.Product.Id} AND \"AvailableQuantity\" >= {line.Quantity}", ct);
            if (affected == 0) throw new BusinessRuleException($"Product '{line.Product.Name}' does not have enough stock.");
            db.OrderDetails.Add(new OrderDetail { OrderId = order.Id, ProductId = line.Product.Id, Quantity = line.Quantity, UnitPrice = line.Product.Price });
        }
        if (cartItems is not null) db.CartItems.RemoveRange(cartItems);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        await notifier.OrderPlacedAsync(order.Id, ct);
        return order.Id;
    }

    private IQueryable<Order> Query() => db.Orders.AsNoTracking().Include(x => x.User).Include(x => x.OrderStatus)
        .Include(x => x.OrderDetails).ThenInclude(x => x.Product);

    private static OrderDto Map(Order x) => new(x.Id, x.UserId, x.User.FullName, x.User.Email, x.OrderStatusId,
        x.OrderStatus.Name, x.OrderDate, x.TotalAmount, x.ShippingAddress,
        x.OrderDetails.Select(d => new OrderDetailDto(d.Id, d.ProductId, d.Product.Name, d.Product.ImageUrl, d.Quantity, d.UnitPrice)).ToList());
}
