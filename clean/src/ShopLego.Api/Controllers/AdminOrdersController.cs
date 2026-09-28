using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize(Roles = "Admin"), ApiController, Route("api/Admin/orders")]
public sealed class AdminOrdersController(IOrderService orders) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await orders.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct) => await orders.GetAsync(id, ct) is { } v ? Ok(v) : NotFound();
    [HttpPut("{id:int}/status")] public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest r, CancellationToken ct) => await orders.UpdateStatusAsync(id, r.OrderStatusId, ct) ? Ok(new { message = "Order status updated successfully" }) : NotFound();
}
