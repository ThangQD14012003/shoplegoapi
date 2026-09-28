using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record LoginRequest([property: Required, EmailAddress] string Email, [property: Required] string Password);
