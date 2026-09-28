using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[ApiController, Route("api/Product")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet] public async Task<IReadOnlyList<ProductDto>> GetAll(CancellationToken ct) => await service.GetAllAsync(ct);
    [HttpGet("{id:int}")] public async Task<ActionResult<ProductDto>> Get(int id, CancellationToken ct) => await service.GetAsync(id, ct) is { } v ? Ok(v) : NotFound(new { message = "Product not found" });
    [Authorize(Roles = "Admin"), HttpPost] public async Task<IActionResult> Create(ProductRequest r, CancellationToken ct) => Ok(new { message = "Product created successfully", productId = await service.CreateAsync(r, ct) });
    [Authorize(Roles = "Admin"), HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, ProductRequest r, CancellationToken ct) => await service.UpdateAsync(id, r, ct) ? Ok(new { message = "Product updated successfully" }) : NotFound();
    [Authorize(Roles = "Admin"), HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await service.DeleteAsync(id, ct) ? Ok(new { message = "Product deleted successfully" }) : NotFound();
}
