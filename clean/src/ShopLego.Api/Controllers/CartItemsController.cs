using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize, ApiController, Route("api/CartItem")]
public sealed class CartItemsController(ICartService service) : ControllerBase
{
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, [FromQuery] int quantity, CancellationToken ct) => await service.SetQuantityAsync(UserId(), id, quantity, ct) ? Ok() : NotFound();
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.RemoveAsync(UserId(), id, ct) ? NoContent() : NotFound();
    private int UserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
