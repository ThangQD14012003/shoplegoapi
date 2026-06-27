using System.ComponentModel.DataAnnotations;

namespace ShopLegoApi.Datas
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalPrice { get; set; }

        public string Status { get; set; } = "Pending";

        // Navigation: 1 Order có nhiều OrderDetail
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
