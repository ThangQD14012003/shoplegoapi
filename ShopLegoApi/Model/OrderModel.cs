namespace ShopLegoApi.Model
{
    public class OrderModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int OrderStatusId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;
    }
}
