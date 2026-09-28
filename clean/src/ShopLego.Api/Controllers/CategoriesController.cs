using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[ApiController, Route("api/Category")]
public sealed class CategoriesController(IReferenceDataService service) : ControllerBase
{ [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await service.GetCategoriesAsync(ct)); }
