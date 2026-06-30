using Microsoft.EntityFrameworkCore;
using ShopLegoApi.Datas;
using ShopLegoApi.DTO;

namespace ShopLegoApi.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly LegoDataContext _context;
        private readonly IEmailService _emailService;

        public OrderRepository(LegoDataContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<List<OrderDto>> GetAll()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => MapToDto(o))
                .ToListAsync();
        }

        public async Task<List<OrderDto>> GetByUserId(int userId)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => MapToDto(o))
                .ToListAsync();
        }

        public async Task<OrderDto?> GetById(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderStatus)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            return order == null ? null : MapToDto(order);
        }

        public async Task<int> PlaceOrderV1(int userId, string shippingAddress)
        {
            // Lấy giỏ hàng của user
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId); // lấy ra cả dữ liệu của CartItems và dữ liệu của Product 

            if (cart == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Giỏ hàng trống hoặc không tồn tại.");

            // Kiểm tra tồn kho
            foreach (var item in cart.CartItems)
            {
                if (item.Product.AvailableQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Sản phẩm '{item.Product.Name}' không đủ hàng. Còn lại: {item.Product.AvailableQuantity}");
            }

            // Tính tổng tiền
            var totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price); // giá trị của lấy được dữ liệu của Product 

            // Tạo đơn hàng
            var order = new Order
            {
                UserId = userId,
                OrderStatusId = 1, // 1 = Pending
                OrderDate = DateTime.UtcNow,
                TotalAmount = totalAmount,
                ShippingAddress = shippingAddress
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            // Tạo OrderDetails và cập nhật tồn kho
            foreach (var item in cart.CartItems)
            {
                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };
                await _context.OrderDetails.AddAsync(detail);

                // Giảm tồn kho khả dụng
                item.Product.AvailableQuantity -= item.Quantity;
                _context.Products.Update(item.Product);
            }

            // Xóa toàn bộ CartItems sau khi đặt hàng
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // Gửi email xác nhận đặt hàng thành công cho Khách hàng, Quản lý, Kế toán
            await _emailService.SendOrderConfirmationAsync(order.Id);

            return order.Id;
        }


        public async Task<int> PlaceOrder(int userId, string shippingAddress)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Lấy giỏ hàng
                var cart = await _context.Carts
                    .Include(c => c.CartItems)
                        .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.CartItems.Any())
                    throw new InvalidOperationException("Giỏ hàng trống hoặc không tồn tại.");

                // Tính tổng tiền
                decimal totalAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.Price);

                // Tạo Order
                var order = new Order
                {
                    UserId = userId,
                    OrderStatusId = 1,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    ShippingAddress = shippingAddress
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                // Tạo OrderDetail + Atomic Update tồn kho
                foreach (var item in cart.CartItems)
                {
                    var affectedRows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
                UPDATE Products
                SET AvailableQuantity = AvailableQuantity - {item.Quantity}
                WHERE Id = {item.ProductId}
                AND AvailableQuantity >= {item.Quantity}");

                    if (affectedRows == 0)
                    {
                        throw new InvalidOperationException(
                            $"Sản phẩm '{item.Product.Name}' không đủ hàng.");
                    }

                    var detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price
                    };

                    await _context.OrderDetails.AddAsync(detail);
                }

                // Xóa giỏ hàng
                _context.CartItems.RemoveRange(cart.CartItems);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Gửi email sau khi Commit thành công
                await _emailService.SendOrderConfirmationAsync(order.Id);

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //public async Task<int> BuyNow(int userId, int productId, int quantity, string shippingAddress)
        //{
        //    // Lấy thông tin sản phẩm
        //    var product = await _context.Products.FindAsync(productId);
        //    if (product == null)
        //        throw new InvalidOperationException("Sản phẩm không tồn tại.");

        //    if (product.AvailableQuantity < quantity)
        //        throw new InvalidOperationException(
        //            $"Sản phẩm '{product.Name}' không đủ hàng. Còn lại: {product.AvailableQuantity}");

        //    // Tính tổng tiền
        //    var totalAmount = product.Price * quantity;

        //    // Tạo đơn hàng
        //    var order = new Order
        //    {
        //        UserId = userId,
        //        OrderStatusId = 1, // 1 = Pending
        //        OrderDate = DateTime.UtcNow,
        //        TotalAmount = totalAmount,
        //        ShippingAddress = shippingAddress
        //    };

        //    await _context.Orders.AddAsync(order);
        //    await _context.SaveChangesAsync();

        //    // Tạo OrderDetail
        //    var detail = new OrderDetail
        //    {
        //        OrderId = order.Id,
        //        ProductId = productId,
        //        Quantity = quantity,
        //        UnitPrice = product.Price
        //    };
        //    await _context.OrderDetails.AddAsync(detail);

        //    // Giảm tồn kho khả dụng
        //    product.AvailableQuantity -= quantity;
        //    _context.Products.Update(product);

        //    await _context.SaveChangesAsync();

        //    // Gửi email xác nhận
        //    await _emailService.SendOrderConfirmationAsync(order.Id);

        //    return order.Id;
        //}

        public async Task<int> BuyNow(int userId, int productId, int quantity, string shippingAddress)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var product = await _context.Products.FindAsync(productId);

                if (product == null)
                    throw new InvalidOperationException("Sản phẩm không tồn tại.");

                // Atomic Update
                var affectedRows = await _context.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE Products
            SET AvailableQuantity = AvailableQuantity - {quantity}
            WHERE Id = {productId}
            AND AvailableQuantity >= {quantity}");

                if (affectedRows == 0)
                {
                    throw new InvalidOperationException(
                        $"Sản phẩm '{product.Name}' không đủ hàng.");
                }

                decimal totalAmount = product.Price * quantity;

                var order = new Order
                {
                    UserId = userId,
                    OrderStatusId = 1,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    ShippingAddress = shippingAddress
                };

                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();

                var detail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };

                await _context.OrderDetails.AddAsync(detail);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // Gửi email sau khi Commit
                await _emailService.SendOrderConfirmationAsync(order.Id);

                return order.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }



        public async Task UpdateStatus(int orderId, int orderStatusId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return;

            // Nếu trạng thái mới là Hoàn thành (4) và đơn hàng chưa ở trạng thái Hoàn thành trước đó
            if (orderStatusId == 4 && order.OrderStatusId != 4)
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Product != null)
                    {
                        detail.Product.StockQuantity -= detail.Quantity;
                        _context.Products.Update(detail.Product);
                    }
                }
            }
            // Nếu trạng thái mới là Hủy (5) và đơn hàng chưa ở trạng thái Hủy trước đó
            else if (orderStatusId == 5 && order.OrderStatusId != 5)
            {
                foreach (var detail in order.OrderDetails)
                {
                    if (detail.Product != null)
                    {
                        detail.Product.AvailableQuantity += detail.Quantity;
                        _context.Products.Update(detail.Product);
                    }
                }
            }

            order.OrderStatusId = orderStatusId;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        // chuyển Order entity sang OrderDto
        private static OrderDto MapToDto(Order o) => new OrderDto
        {
            Id = o.Id,
            UserId = o.UserId,
            UserFullName = o.User?.FullName ?? string.Empty,
            UserEmail = o.User?.Email ?? string.Empty,
            OrderStatusId = o.OrderStatusId,
            OrderStatusName = o.OrderStatus?.Name ?? string.Empty,
            OrderDate = o.OrderDate,
            TotalAmount = o.TotalAmount,
            ShippingAddress = o.ShippingAddress,
            OrderDetails = o.OrderDetails.Select(od => new OrderDetailDto
            {
                Id = od.Id,
                ProductId = od.ProductId,
                ProductName = od.Product?.Name ?? string.Empty,
                ProductImage = od.Product?.ImageUrl ?? string.Empty,
                Quantity = od.Quantity,
                UnitPrice = od.UnitPrice
            }).ToList()
        };
    }
}
