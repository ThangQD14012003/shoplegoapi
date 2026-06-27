using System.ComponentModel.DataAnnotations;

namespace ShopLegoApi.Datas
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Image { get; set; } = string.Empty;


        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
