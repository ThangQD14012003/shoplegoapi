using ShopLego.Domain.Entities;
namespace ShopLego.Application;

public interface ITokenService
{
    AuthTokens Create(User user);
    int? ReadUserId(string refreshToken);
}
