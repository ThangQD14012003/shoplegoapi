namespace ShopLego.Application;

public sealed record OrderDto(int Id, int UserId, string UserFullName, string UserEmail, int OrderStatusId,
    string OrderStatusName, DateTime OrderDate, decimal TotalAmount, string ShippingAddress,
    IReadOnlyList<OrderDetailDto> OrderDetails);
