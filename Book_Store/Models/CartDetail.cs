using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Store.Models
{
    [Table("CartDetail")]
    public class CartDetail
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ShoppingCartId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        [ForeignKey("ShoppingCartId")]
        public ShoppingCart ShoppingCart { get; set; }
    }
}
