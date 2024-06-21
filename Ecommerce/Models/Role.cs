using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models;

public partial class Role
{
    public int Id { get; set; }
    [Required]
    public string? Value { get; set; }
    [Required]
    public string? Title { get; set; }
}
