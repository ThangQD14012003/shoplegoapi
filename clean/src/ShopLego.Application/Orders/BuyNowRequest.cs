using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record BuyNowRequest([Range(1, int.MaxValue)] int ProductId,
    [Range(1, int.MaxValue)] int Quantity, [Required] string ShippingAddress);
