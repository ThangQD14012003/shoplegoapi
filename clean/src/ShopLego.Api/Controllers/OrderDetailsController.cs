using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize(Roles = "Admin"), ApiController, Route("api/OrderDetail")]
public sealed class OrderDetailsController(IReferenceDataService service) : ControllerBase
{ [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct) => await service.GetOrderDetailAsync(id, ct) is { } v ? Ok(v) : NotFound(); }
