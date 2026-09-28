using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize(Roles = "Admin"), ApiController, Route("api/EmailLog")]
public sealed class EmailLogsController(IReferenceDataService service) : ControllerBase
{ [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetEmailLogsAsync(ct)); }
