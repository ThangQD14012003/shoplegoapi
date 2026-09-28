namespace ShopLego.Application;

public sealed record OrderDetailDto(int Id, int ProductId, string ProductName, string ProductImage, int Quantity, decimal UnitPrice)
{
    public decimal SubTotal => Quantity * UnitPrice;
}
