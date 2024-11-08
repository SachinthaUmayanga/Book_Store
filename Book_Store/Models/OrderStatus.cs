using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Store.Models
{
    [Table("OrderStatus")]
    public class OrderStatus
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string? StatusName { get; set; }
    }

}
