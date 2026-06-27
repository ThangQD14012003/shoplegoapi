using System.ComponentModel.DataAnnotations;

namespace ShopLegoApi.Datas
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public Customer? Customer { get; set; }

        public Product? Product { get; set; }
    }
}
