using System.ComponentModel.DataAnnotations;

namespace Book_Store.Models.DTOs;

public class CheckoutModel
{
    [Required]
    [MaxLength(30)]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    [Required]
    public string? MobileNumber { get; set; }

    [Required]
    [MaxLength(100)]
    public string? Address { get; set; }

    [Required]
    [MaxLength(30)]
    public string? PaymentMethod { get; set; }
}
