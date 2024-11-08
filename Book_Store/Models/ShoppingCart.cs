using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Store.Models
{
    [Table("ShoppingCart")]
    public class ShoppingCart
    {
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public bool IsDeleted { get; set; } = false;


    }
}
