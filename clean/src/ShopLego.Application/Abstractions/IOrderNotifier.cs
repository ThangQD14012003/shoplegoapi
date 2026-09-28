namespace ShopLego.Application;

public interface IOrderNotifier
{
    Task OrderPlacedAsync(int orderId, CancellationToken ct);
}
