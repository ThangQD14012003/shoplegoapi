using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record LoginRequest([Required, EmailAddress] string Email, [Required] string Password);
