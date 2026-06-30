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

        public UserController(IUserRepository userRepository)
        {
            _userRepo = userRepository;
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

            var success = await _userRepo.Login(model.Email, model.Password);
            if (!success)
                return BadRequest(new { message = "Invalid email or password" });

            var user = await _userRepo.GetByEmail(model.Email);
            return Ok(new
            {
                message = "Login successful",
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
