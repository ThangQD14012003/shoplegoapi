using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ShopLegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;

        private static readonly HashSet<string> AllowedExtensions =
            new(StringComparer.OrdinalIgnoreCase) { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public UploadController(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        /// <summary>Upload ảnh sản phẩm, lưu vào wwwroot/uploads/products/</summary>
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Không có file được gửi lên." });

            if (file.Length > MaxFileSizeBytes)
                return BadRequest(new { message = "File quá lớn. Giới hạn 5MB." });

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(new { message = $"Định dạng không được hỗ trợ. Chỉ chấp nhận: {string.Join(", ", AllowedExtensions)}" });

            // Tạo tên file độc nhất
            var newFileName = $"{Guid.NewGuid()}{ext}";
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");

            // Đảm bảo thư mục tồn tại
            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, newFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Build URL tuyệt đối trả về cho client
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var imageUrl = $"{baseUrl}/uploads/products/{newFileName}";

            return Ok(new { imageUrl });
        }
    }
}
