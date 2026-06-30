using System.ComponentModel.DataAnnotations;

namespace ShopLegoApi.Datas
{
    public class SystemSetting
    {
        [Key]
        public string Key { get; set; } = string.Empty;

        [Required]
        public string Value { get; set; } = string.Empty;
    }
}
