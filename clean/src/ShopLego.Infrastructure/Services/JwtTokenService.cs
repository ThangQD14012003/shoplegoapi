using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopLego.Application;
using ShopLego.Domain.Entities;
namespace ShopLego.Infrastructure.Services;

public sealed class JwtTokenService(IConfiguration config) : ITokenService
{
    public AuthTokens Create(User u) => new(CreateToken(u, TimeSpan.FromMinutes(GetDouble("Jwt:ExpireMinutes", 15)), "access"), CreateToken(u, TimeSpan.FromDays(GetDouble("Jwt:RefreshTokenExpireDays", 7)), "refresh"));
    public int? ReadUserId(string token) { try { var p = new JwtSecurityTokenHandler().ValidateToken(token, Parameters(), out var v); if (v is not JwtSecurityToken jwt || jwt.Header.Alg != SecurityAlgorithms.HmacSha256 || p.FindFirst("token_type")?.Value != "refresh") return null; return int.TryParse(p.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null; } catch { return null; } }
    private string CreateToken(User u, TimeSpan life, string type) { Claim[] claims = [new(ClaimTypes.NameIdentifier, u.Id.ToString()), new(ClaimTypes.Email, u.Email), new(ClaimTypes.Role, u.Role), new("token_type", type)]; var jwt = new JwtSecurityToken(config["Jwt:Issuer"], config["Jwt:Audience"], claims, expires: DateTime.UtcNow.Add(life), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256)); return new JwtSecurityTokenHandler().WriteToken(jwt); }
    private TokenValidationParameters Parameters() => new() { ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true, ValidIssuer = config["Jwt:Issuer"], ValidAudience = config["Jwt:Audience"], IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), ClockSkew = TimeSpan.Zero };
    private double GetDouble(string key, double fallback) => double.TryParse(config[key], out var v) ? v : fallback;
}
