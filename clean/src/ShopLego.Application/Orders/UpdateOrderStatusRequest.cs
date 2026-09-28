using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record UpdateOrderStatusRequest([Range(1, 5)] int OrderStatusId);
