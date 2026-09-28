namespace ShopLego.Application;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct);
    Task<ProductDto?> GetAsync(int id, CancellationToken ct);
    Task<int> CreateAsync(ProductRequest request, CancellationToken ct);
    Task<bool> UpdateAsync(int id, ProductRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
}
