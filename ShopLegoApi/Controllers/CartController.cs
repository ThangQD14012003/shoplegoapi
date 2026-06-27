using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShopLegoApi.Services;

namespace ShopLegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartItemRepository _cartRepo;

        public CartController(ICartItemRepository cartRepository)
        {
            _cartRepo = cartRepository;
        }

        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCartByCustomerId(int customerId)
        {
            var result = await _cartRepo.GetCartByCustomerId(customerId);
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromQuery] int customerId, [FromQuery] int productId)
        {
            await _cartRepo.AddToCart(customerId, productId);
            return Ok(new { message = "Added to cart successfully" });
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            await _cartRepo.DeleteCartItem(cartItemId);
            return Ok(new { message = "Deleted cart item" });
        }

        [HttpDelete("clear/{customerId}")]
        public IActionResult ClearCart(int customerId)
        {
            return StatusCode(501, new { message = "Not implemented yet" });
        }

        [HttpPut("update/{cartItemId}")]
        public IActionResult UpdateQuantity(int cartItemId)
        {
            return StatusCode(501, new { message = "Not implemented yet" });
        }
    }
}