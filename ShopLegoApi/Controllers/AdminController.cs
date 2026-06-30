using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopLegoApi.Datas;
using ShopLegoApi.Model;
using ShopLegoApi.Services;

namespace ShopLegoApi.Controllers
{
    
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly LegoDataContext _context;

        public AdminController(
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            LegoDataContext context)
        {
            _productRepo = productRepository;
            _orderRepo = orderRepository;
            _context = context;
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepo.GetAll();
            return Ok(products);
        }

        [HttpGet("products/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepo.GetById(id);
            if (product == null) return NotFound(new { message = "Product not found" });
            return Ok(product);
        }

        [HttpPost("products")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _productRepo.Add(model);
            return Ok(new { message = "Product created successfully", productId = id });
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductModel model)
        {
            var existing = await _productRepo.GetById(id);
            if (existing == null) return NotFound(new { message = "Product not found" });

            await _productRepo.Update(model, id);
            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete("products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var existing = await _productRepo.GetById(id);
            if (existing == null) return NotFound(new { message = "Product not found" });

            await _productRepo.DeleteById(id);
            return Ok(new { message = "Product deleted successfully" });
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepo.GetAll();
            return Ok(orders);
        }

        [HttpGet("orders/{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderRepo.GetById(id);
            if (order == null) return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

      
        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] UpdateOrderStatusRequest request)
        {
            var order = await _orderRepo.GetById(orderId);
            if (order == null) return NotFound(new { message = "Order not found" });

            if (request.OrderStatusId < 1 || request.OrderStatusId > 5)
                return BadRequest(new { message = "Invalid order status id (1-5)" });

            await _orderRepo.UpdateStatus(orderId, request.OrderStatusId);
            return Ok(new { message = "Order status updated successfully" });
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            var managerEmail = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "ManagerEmail");
            var accountantEmail = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "AccountantEmail");

            return Ok(new
            {
                ManagerEmail = managerEmail?.Value ?? "manager@example.com",
                AccountantEmail = accountantEmail?.Value ?? "accountant@example.com"
            });
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var managerEmail = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "ManagerEmail");
            if (managerEmail == null)
            {
                managerEmail = new SystemSetting { Key = "ManagerEmail", Value = request.ManagerEmail };
                _context.SystemSettings.Add(managerEmail);
            }
            else
            {
                managerEmail.Value = request.ManagerEmail;
                _context.SystemSettings.Update(managerEmail);
            }

            var accountantEmail = await _context.SystemSettings
                .FirstOrDefaultAsync(s => s.Key == "AccountantEmail");
            if (accountantEmail == null)
            {
                accountantEmail = new SystemSetting { Key = "AccountantEmail", Value = request.AccountantEmail };
                _context.SystemSettings.Add(accountantEmail);
            }
            else
            {
                accountantEmail.Value = request.AccountantEmail;
                _context.SystemSettings.Update(accountantEmail);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Settings updated successfully" });
        }
    }

    /// <summary>Request body cho endpoint cập nhật cấu hình email</summary>
    public class UpdateSettingsRequest
    {
        public string ManagerEmail { get; set; } = string.Empty;
        public string AccountantEmail { get; set; } = string.Empty;
    }

    /// <summary>Request body cho endpoint cập nhật trạng thái đơn hàng</summary>
    public class UpdateOrderStatusRequest
    {
        public int OrderStatusId { get; set; }
    }
}
