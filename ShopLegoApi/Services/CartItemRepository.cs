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

        public async Task AddToCart(int userId, int productId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Carts.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(x => x.CartId == cart.Id
                                       && x.ProductId == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                var product = await _context.Products.FindAsync(productId);
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = product?.Price ?? 0
                };

                await _context.CartItems.AddAsync(cartItem);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearCart(int userId)
        {
            var cartItems = await _context.CartItems
                .Where(x => x.Cart.UserId == userId)
                .ToListAsync();

            if (cartItems.Any())
            {
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteCartItem(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return cartItem.Id;
            }
            return -1;
        }

        public async Task<List<CartItemDto>> GetCartByUserId(int userId)
        {
            return await _context.CartItems
                .Where(x => x.Cart.UserId == userId)
                .Select(x => new CartItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    Quantity = x.Quantity,
                    ProductName = x.Product.Name,
                    Price = x.Product.Price,
                    Image = x.Product.ImageUrl
                })
                .ToListAsync();
        }

        public async Task UpdateQuantity(int cartItemId, int quantity)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _context.CartItems.Update(cartItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}
