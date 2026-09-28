using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize, ApiController, Route("api/Order")]
public sealed class OrdersController(IOrderService service) : ControllerBase
{
    [HttpGet("{userId:int}")] public async Task<IActionResult> ByUser(int userId, CancellationToken ct) { if (!Can(userId)) return Forbid(); return Ok(await service.GetByUserAsync(userId, ct)); }
    [HttpGet("detail/{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct) { var v = await service.GetAsync(id, ct); if (v is null) return NotFound(); if (!Can(v.UserId)) return Forbid(); return Ok(v); }
    [HttpPost("place")] public async Task<IActionResult> Place(PlaceOrderRequest r, CancellationToken ct) => Ok(new { message = "Order placed successfully", orderId = await service.PlaceAsync(Uid(), r.ShippingAddress, ct) });
    [HttpPost("buy-now")] public async Task<IActionResult> Buy(BuyNowRequest r, CancellationToken ct) => Ok(new { message = "Mua hàng thành công", orderId = await service.BuyNowAsync(Uid(), r.ProductId, r.Quantity, r.ShippingAddress, ct) });
    private int Uid() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); private bool Can(int id) => Uid() == id || User.IsInRole("Admin");
}
