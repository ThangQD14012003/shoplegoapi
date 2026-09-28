using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize(Roles = "Admin"), ApiController, Route("api/Admin/settings")]
public sealed class SystemSettingsController(ISettingsService settings) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> Get(CancellationToken ct) => Ok(await settings.GetAsync(ct));
    [HttpPut] public async Task<IActionResult> Update(SettingsDto r, CancellationToken ct) { await settings.UpdateAsync(r, ct); return Ok(new { message = "Settings updated successfully" }); }
}
