using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Store.Models
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public Guid Id { get; set; }

        [Required]
        public Guid OrderId { get; set; }

        [Required]
        public Guid BookId { get; set; }

        [Required]
        public int Quanity { get; set; }

        [Required]
        public double UnitPrice { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }
    }
}
