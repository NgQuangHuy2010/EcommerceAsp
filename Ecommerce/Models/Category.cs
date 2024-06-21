using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Ecommerce.Models;

public partial class Category
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Mày nhập tên vô đi")]
    public string? Name { get; set; }

    public string? Image { get; set; }
    [NotMapped]
    [Required(ErrorMessage = "Thiếu hình kìa??")]
    public IFormFile? NameImage { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
