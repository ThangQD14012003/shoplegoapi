namespace ShopLegoApi.Services
{
    public class EmailQueueMessage
    {
        public int OrderId { get; set; }
        public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
    }
}
