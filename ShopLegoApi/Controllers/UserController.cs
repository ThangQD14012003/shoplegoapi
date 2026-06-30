using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopLegoApi.Model;
using ShopLegoApi.Services;

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

            var token = _jwtService.GenerateToken(user);
            return Ok(new
            {
                message = "Login successful",
                token,      
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
    }
}
