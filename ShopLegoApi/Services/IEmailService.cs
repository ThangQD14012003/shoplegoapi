namespace ShopLegoApi.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(int orderId);
    }
}
