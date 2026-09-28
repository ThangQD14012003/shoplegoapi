namespace ShopLego.Domain.Entities;

public sealed class OrderStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Order> Orders { get; set; } = [];
}
