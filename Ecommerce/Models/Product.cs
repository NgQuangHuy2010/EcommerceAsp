using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Models;

public partial class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn danh mục cho sản phẩm !!")]
    public int? IdCategory { get; set; }

    [Required(ErrorMessage = "Vui lòng thêm tên sản phẩm")]
    public string? NameProduct { get; set; }

    public string? ImageProduct { get; set; }

    [NotMapped]
    [Required(ErrorMessage = "Vui lòng thêm hình cho sản phẩm này")]
    public IFormFile? NameImage { get; set; }

    [Required(ErrorMessage = "Vui lòng thêm giá cho sản phẩm này")]
    public decimal? PriceProduct { get; set; }

    public decimal? Discount { get; set; }

    public string? Description { get; set; }

    public int? Status { get; set; }

    public string? Model { get; set; }

    public string? Producer { get; set; }

    public string? Origin { get; set; }

    public virtual Category? IdCategoryNavigation { get; set; }
}
