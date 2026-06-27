using System.ComponentModel.DataAnnotations;

namespace ShopLegoApi.Datas
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
