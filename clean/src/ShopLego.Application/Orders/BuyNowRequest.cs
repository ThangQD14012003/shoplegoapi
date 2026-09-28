using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record BuyNowRequest([property: Range(1, int.MaxValue)] int ProductId,
    [property: Range(1, int.MaxValue)] int Quantity, [property: Required] string ShippingAddress);
