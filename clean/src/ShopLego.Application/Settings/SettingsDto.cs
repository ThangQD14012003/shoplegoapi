using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record SettingsDto([Required, EmailAddress] string ManagerEmail,
    [Required, EmailAddress] string AccountantEmail);
