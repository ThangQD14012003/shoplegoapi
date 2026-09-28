namespace ShopLego.Application;

public sealed record CartItemDto(int Id, int ProductId, int Quantity, string ProductName, decimal Price, string Image);
