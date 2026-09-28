namespace ShopLego.Domain.Entities;

public sealed class EmailLog
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string ReceiverEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateTime SendTime { get; set; }
    public bool Status { get; set; }
    public Order Order { get; set; } = null!;
}
