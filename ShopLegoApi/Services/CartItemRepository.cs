using Microsoft.EntityFrameworkCore;
using ShopLegoApi.Datas;
using ShopLegoApi.DTO;

namespace ShopLegoApi.Services
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly LegoDataContext _context;

        public CartItemRepository(LegoDataContext context)
        {
            _context = context;
        }
        public async Task AddToCart(int customerId, int productId)
        {
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.CustomerId == customerId
                                       && x.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                cartItem = new CartItem
                {
                    CustomerId = customerId,
                    ProductId = productId,
                    Quantity = 1
                };

                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public Task ClearCart(int customerId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteCartItem(int cartItemId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(o => o.Id == cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

        }

        public async Task<List<CartItemDto>> GetCartByCustomerId(int customerId)
        {
            return await _context.CartItems
                .Where(x => x.CustomerId == customerId)
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    Image = x.Product.Image
                })
                .ToListAsync();
        }

        public Task UpdateQuantity(int cartItemId)
        {
            throw new NotImplementedException();
        }
    }
}
