using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize, ApiController, Route("api/Cart")]
public sealed class CartsController(ICartService service) : ControllerBase
{
    [HttpGet("customer/{userId:int}")] public async Task<IActionResult> Get(int userId, CancellationToken ct) { if (!Can(userId)) return Forbid(); return Ok(await service.GetAsync(userId, ct)); }
    [HttpPost("add")] public async Task<IActionResult> Add([FromQuery] int customerId, [FromQuery] int productId, CancellationToken ct) { if (!Can(customerId)) return Forbid(); await service.AddAsync(customerId, productId, ct); return Ok(new { message = "Added to cart successfully" }); }
    [HttpDelete("{id:int}")] public async Task<IActionResult> Remove(int id, CancellationToken ct) => await service.RemoveAsync(Uid(), id, ct) ? Ok(new { id }) : NotFound();
    [HttpDelete("clear/{userId:int}")] public async Task<IActionResult> Clear(int userId, CancellationToken ct) { if (!Can(userId)) return Forbid(); await service.ClearAsync(userId, ct); return Ok(new { message = "Cart cleared successfully" }); }
    [HttpPut("update/{id:int}")] public async Task<IActionResult> Update(int id, [FromQuery] int quantity, CancellationToken ct) => await service.SetQuantityAsync(Uid(), id, quantity, ct) ? Ok(new { message = "Updated quantity successfully" }) : NotFound();
    private int Uid() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); private bool Can(int id) => Uid() == id || User.IsInRole("Admin");
}
