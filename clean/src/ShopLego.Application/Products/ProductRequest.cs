using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record ProductRequest([Range(1, int.MaxValue)] int CategoryId,
    [Required, MaxLength(200)] string Name, [Required] string Description,
    [Range(0, double.MaxValue)] decimal Price, [Range(0, int.MaxValue)] int StockQuantity,
    [Range(0, int.MaxValue)] int AvailableQuantity, [Required] string ImageUrl);
