using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public partial class Account
{
    public int Id { get; set; }

    public string? Fullname { get; set; }
    [Required]
    [EmailAddress]

    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Dob { get; set; }

    public string? Gender { get; set; }

    public string? Status { get; set; }

    public int? Role { get; set; }
}
