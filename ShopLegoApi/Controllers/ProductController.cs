using Microsoft.AspNetCore.Mvc;
using ShopLegoApi.Model;
using ShopLegoApi.Services;

namespace ShopLegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _IPro;

        public ProductController(IProductRepository productRepository)
        {
            _IPro = productRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _IPro.GetAll();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _IPro.GetById(id);

            if (product == null)
                return NotFound("Product not found");

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductModel model)
        {
            if (model == null)
                return BadRequest("Invalid data");

            var id = await _IPro.Add(model);

            return Ok(new
            {
                message = "Product created successfully",
                productId = id
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductModel model)
        {
            var product = await _IPro.GetById(id);

            if (product == null)
                return NotFound("Product not found");

            await _IPro.Update(model, id);

            return Ok(new
            {
                message = "Product updated successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _IPro.GetById(id);

            if (product == null)
                return NotFound("Product not found");

            await _IPro.DeleteById(id);

            return Ok(new
            {
                message = "Product deleted successfully"
            });
        }
    }
}