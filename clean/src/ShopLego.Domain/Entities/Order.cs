namespace ShopLego.Domain.Entities;

public sealed class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OrderStatusId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public User User { get; set; } = null!;
    public OrderStatus OrderStatus { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = [];
    public ICollection<EmailLog> EmailLogs { get; set; } = [];
}
