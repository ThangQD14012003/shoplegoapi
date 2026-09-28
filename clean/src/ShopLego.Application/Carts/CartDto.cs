namespace ShopLego.Application;

public sealed record CartDto(int Id, int UserId, DateTime CreatedAt, IReadOnlyList<CartItemDto> Items);
