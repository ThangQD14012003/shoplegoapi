using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record SettingsDto([property: Required, EmailAddress] string ManagerEmail,
    [property: Required, EmailAddress] string AccountantEmail);
