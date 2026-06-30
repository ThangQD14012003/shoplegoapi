using ShopLegoApi.DTO;

namespace ShopLegoApi.Services
{
    public interface IOrderRepository
    {
        // Admin: lấy tất cả đơn hàng
        Task<List<OrderDto>> GetAll();

        // User: lấy đơn hàng theo userId
        Task<List<OrderDto>> GetByUserId(int userId);

        // Lấy chi tiết một đơn hàng
        Task<OrderDto?> GetById(int orderId);

        // User: đặt hàng từ giỏ hàng (tạo Order, OrderDetails, giảm tồn kho, xóa Cart)
        Task<int> PlaceOrder(int userId, string shippingAddress);

        // Admin: cập nhật trạng thái đơn hàng
        Task UpdateStatus(int orderId, int orderStatusId);
    }
}
