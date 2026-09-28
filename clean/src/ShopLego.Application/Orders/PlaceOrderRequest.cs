using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record PlaceOrderRequest([property: Required] string ShippingAddress);
