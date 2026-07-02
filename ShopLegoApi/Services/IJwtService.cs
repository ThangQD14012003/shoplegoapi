using ShopLegoApi.Model;
using System.Security.Claims;

namespace ShopLegoApi.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(UserModel user);
        string GenerateRefreshToken(UserModel user);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}