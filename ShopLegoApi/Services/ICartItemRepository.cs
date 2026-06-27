using ShopLegoApi.Datas;
using ShopLegoApi.DTO;

namespace ShopLegoApi.Services
{
    public interface ICartItemRepository
    {
        Task AddToCart(int customerId, int productId);

        Task<List<CartItemDto>> GetCartByCustomerId(int customerId);

        Task UpdateQuantity(int cartItemId);

        Task DeleteCartItem(int cartItemId);

        Task ClearCart(int customerId);
    }
}
