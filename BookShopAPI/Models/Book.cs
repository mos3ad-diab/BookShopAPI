using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopAPI.Models
{
    public class Book
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        [ForeignKey(nameof(AuthorId))]
        public Author Author { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public int? Rate { get; set; }
        public int? Review { get; set; }
        public DateOnly Year { get; set; }
        public decimal Price { get; set; }
        public int Amount { get; set; } = 0;
        public int Sold { get; set; } = 0;
        public decimal Discount { get; set; }

        
    }
}
