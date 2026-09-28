using Microsoft.AspNetCore.Mvc;
using ShopLego.Application;
namespace ShopLego.Api.Controllers;

[ApiController, Route("api/User")]
public sealed class UsersController(IIdentityService service) : ControllerBase
{
    [HttpPost("register")] public async Task<IActionResult> Register(RegisterRequest r, CancellationToken ct) => Ok(new { message = "User registered successfully", userId = await service.RegisterAsync(r, ct) });
    [HttpPost("login")] public async Task<IActionResult> Login(LoginRequest r, CancellationToken ct) => await service.LoginAsync(r, ct) is { } v ? Ok(new { message = "Login successful", v.Token, v.RefreshToken, v.User }) : BadRequest(new { message = "Invalid email or password" });
    [HttpPost("refresh")] public async Task<IActionResult> Refresh(RefreshTokenRequest r, CancellationToken ct) => await service.RefreshAsync(r.RefreshToken, ct) is { } v ? Ok(v) : Unauthorized(new { message = "Invalid refresh token" });
}
