using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record PlaceOrderRequest([Required] string ShippingAddress);
