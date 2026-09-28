using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record RefreshTokenRequest([property: Required] string RefreshToken);
