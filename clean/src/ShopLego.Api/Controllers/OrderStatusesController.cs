using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[ApiController, Route("api/OrderStatus")]
public sealed class OrderStatusesController(IReferenceDataService service) : ControllerBase
{ [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetOrderStatusesAsync(ct)); }
