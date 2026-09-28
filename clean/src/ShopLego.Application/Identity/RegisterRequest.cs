using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record RegisterRequest([Required, MaxLength(200)] string FullName,
    [Required, EmailAddress] string Email, [Required, MinLength(8)] string Password,
    [Required] string Phone, [Required] string Address);
