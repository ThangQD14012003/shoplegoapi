using System.ComponentModel.DataAnnotations;
namespace ShopLego.Application;

public sealed record ProductRequest([property: Range(1, int.MaxValue)] int CategoryId,
    [property: Required, MaxLength(200)] string Name, [property: Required] string Description,
    [property: Range(0, double.MaxValue)] decimal Price, [property: Range(0, int.MaxValue)] int StockQuantity,
    [property: Range(0, int.MaxValue)] int AvailableQuantity, [property: Required] string ImageUrl);
