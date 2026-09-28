using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record RegisterRequest([property: Required, MaxLength(200)] string FullName,
    [property: Required, EmailAddress] string Email, [property: Required, MinLength(8)] string Password,
    [property: Required] string Phone, [property: Required] string Address);
