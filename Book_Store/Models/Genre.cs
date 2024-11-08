using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Book_Store.Models
{
    [Table("Genre")]
    public class Genre
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string GenreName { get; set; }

        public List<Book> Books { get; set; }
    }
}
