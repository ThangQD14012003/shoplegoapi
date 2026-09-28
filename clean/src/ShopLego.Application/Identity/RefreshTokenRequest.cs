using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record RefreshTokenRequest([Required] string RefreshToken);
