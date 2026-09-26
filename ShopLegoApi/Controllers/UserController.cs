using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLegoApi.Model;
using ShopLegoApi.Services;
using System.Security.Claims;

namespace ShopLegoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _jwtService;

        public UserController(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepo = userRepository;
            _jwtService = jwtService; 
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _userRepo.GetByEmail(model.Email);
            if (existingUser != null)
                return BadRequest(new { message = "Email already registered" });

            var userId = await _userRepo.Register(model);
            return Ok(new { message = "User registered successfully", userId = userId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userRepo.Login(model.Email, model.Password);
            if (user == null)
                return BadRequest(new { message = "Invalid email or password" });

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            return Ok(new
            {
                message = "Login successful",
                token = accessToken,
                refreshToken,
                user = new
                {
                    id = user?.Id,
                    fullName = user?.FullName,
                    email = user?.Email,
                    role = user?.Role, 
                    address = user?.Address
                }
            });
        }

        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new { message = "Refresh token is required" });

            var principal = _jwtService.GetPrincipalFromExpiredToken(request.RefreshToken);
            if (principal == null)
                return Unauthorized(new { message = "Invalid refresh token" });

            var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                return Unauthorized(new { message = "Invalid refresh token" });

            var user = _userRepo.GetById(userId).Result;
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            var userModel = new UserModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Address = user.Address
            };

            var accessToken = _jwtService.GenerateAccessToken(userModel);
            var refreshToken = _jwtService.GenerateRefreshToken(userModel);
            return Ok(new { token = accessToken, refreshToken });
        }
    }
}
