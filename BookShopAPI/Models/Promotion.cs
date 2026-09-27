using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopAPI.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int BookId { get; set; }
        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }

        public float Discount { get; set; }

        public int MaxUsage { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsValid { get; set; } = true;
    }
}
