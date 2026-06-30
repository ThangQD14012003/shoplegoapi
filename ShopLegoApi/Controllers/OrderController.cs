using Microsoft.AspNetCore.Mvc;
using ShopLegoApi.Services;

namespace ShopLegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _orderRepo;

        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepo = orderRepository;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var orders = await _orderRepo.GetByUserId(userId);
            return Ok(orders);
        }

        [HttpGet("detail/{orderId}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            var order = await _orderRepo.GetById(orderId);
            if (order == null) return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

   
        [HttpPost("place")]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var orderId = await _orderRepo.PlaceOrder(request.UserId, request.ShippingAddress);
                return Ok(new { message = "Order placed successfully", orderId = orderId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class PlaceOrderRequest
    {
        public int UserId { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
    }
}
