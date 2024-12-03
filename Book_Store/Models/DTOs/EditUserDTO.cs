using System.ComponentModel.DataAnnotations;

namespace Book_Store.Models.DTOs
{
    public class EditUserDTO
    {
        // The UserId property is used to identify the user being edited
        public string UserId { get; set; }

        // UserName is required and used for the user's name
        [Required(ErrorMessage = "User name is required.")]
        [StringLength(100, ErrorMessage = "User name can't be longer than 100 characters.")]
        public string UserName { get; set; }

        // Email is required and validated to be in a proper email format
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Email can't be longer than 100 characters.")]
        public string Email { get; set; }

        // Role can be selected, but it’s optional and used for updating user roles
        public string Role { get; set; }
    }
}
