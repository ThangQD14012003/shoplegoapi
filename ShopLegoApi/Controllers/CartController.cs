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

        [HttpGet("customer/{userId}")] 
        public async Task<IActionResult> GetCartByUserId(int userId)
        {
            var result = await _cartRepo.GetCartByUserId(userId);
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
            return Ok(await _cartRepo.DeleteCartItem(cartItemId));
        }

        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartRepo.ClearCart(userId);
            return Ok(new { message = "Cart cleared successfully" });
        }

        [HttpPut("update/{cartItemId}")]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, [FromQuery] int quantity)
        {
            await _cartRepo.UpdateQuantity(cartItemId, quantity);
            return Ok(new { message = "Updated quantity successfully" });
        }
    }
}