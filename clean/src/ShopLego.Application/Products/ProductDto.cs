namespace ShopLego.Application;

public sealed record ProductDto(int Id, int CategoryId, string Name, string Description, decimal Price,
    int StockQuantity, int AvailableQuantity, string ImageUrl, DateTime CreatedAt, DateTime UpdatedAt);
