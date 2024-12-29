using System.ComponentModel.DataAnnotations;

namespace Book_Store.Models.DTOs
{
    public class CreateReviewDTO
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(40)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
    }
}
