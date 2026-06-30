using ShopLegoApi.Datas;
using ShopLegoApi.DTO;

namespace ShopLegoApi.Services
{
    public interface ICartItemRepository
    {
        Task AddToCart(int userId, int productId);

        Task<List<CartItemDto>> GetCartByUserId(int userId);

        Task UpdateQuantity(int cartItemId, int quantity);

        Task<int> DeleteCartItem(int cartItemId);

        Task ClearCart(int userId);
    }
}
