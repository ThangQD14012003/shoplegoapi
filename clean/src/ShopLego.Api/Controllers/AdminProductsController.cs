using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[Authorize(Roles = "Admin"), ApiController, Route("api/Admin/products")]
public sealed class AdminProductsController(IProductService products) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> GetAll(CancellationToken ct) => Ok(await products.GetAllAsync(ct));
    [HttpGet("{id:int}")] public async Task<IActionResult> Get(int id, CancellationToken ct) => await products.GetAsync(id, ct) is { } v ? Ok(v) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(ProductRequest r, CancellationToken ct) => Ok(new { productId = await products.CreateAsync(r, ct) });
    [HttpPut("{id:int}")] public async Task<IActionResult> Update(int id, ProductRequest r, CancellationToken ct) => await products.UpdateAsync(id, r, ct) ? Ok() : NotFound();
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id, CancellationToken ct) => await products.DeleteAsync(id, ct) ? Ok() : NotFound();
}
