using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShopLegoApi.Datas
{
    public class EmailLog
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public string ReceiverEmail { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public DateTime SendTime { get; set; }

        public bool Status { get; set; }
    }
}
