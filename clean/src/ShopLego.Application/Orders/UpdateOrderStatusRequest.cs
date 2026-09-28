using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record UpdateOrderStatusRequest([property: Range(1, 5)] int OrderStatusId);
