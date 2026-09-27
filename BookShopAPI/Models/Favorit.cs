using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookShopAPI.Models
{
    [PrimaryKey(nameof(ApplicationUserId) , nameof(BookId))]
    public class Favorit
    {
        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }

        public int BookId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public Book Book { get; set; }
    }
}
