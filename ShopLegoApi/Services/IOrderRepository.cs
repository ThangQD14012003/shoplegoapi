using ShopLegoApi.DTO;

namespace ShopLegoApi.Services
{
    public interface IOrderRepository
    {
        // lấy tất cả đơn hàng
        Task<List<OrderDto>> GetAll();

        // lấy đơn hàng theo userId
        Task<List<OrderDto>> GetByUserId(int userId);

        // Lấy chi tiết một đơn hàng
        Task<OrderDto?> GetById(int orderId);

        // đặt hàng từ giỏ hàng (tạo Order, OrderDetails, giảm tồn kho, xóa Cart)
        Task<int> PlaceOrder(int userId, string shippingAddress);

        // mua ngay 1 sản phẩm (không qua giỏ hàng)
        Task<int> BuyNow(int userId, int productId, int quantity, string shippingAddress);

        // cập nhật trạng thái đơn hàng
        Task UpdateStatus(int orderId, int orderStatusId);
    }
}
