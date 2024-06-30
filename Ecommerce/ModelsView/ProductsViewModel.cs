using Ecommerce.Attributes;
using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.ModelsView
{
    public class ProductsViewModel : IViewModelWithId
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục cho sản phẩm.")]
        public int? IdCategory { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên sản phẩm.")]
        public string? NameProduct { get; set; }

        public string? ImageProduct { get; set; }

        [NotMapped]
        [RequiredIfNew(nameof(Id), ErrorMessage = "Vui lòng thêm hình cho sản phẩm.")]
        public IFormFile? NameImage { get; set; }

        [Required(ErrorMessage = "Vui lòng thêm giá cho sản phẩm.")]
        public decimal? PriceProduct { get; set; }

        public decimal? Discount { get; set; }

        public string? Description { get; set; }

        public int? Status { get; set; }

        public string? Model { get; set; }

        public string? Producer { get; set; }

        public string? Origin { get; set; }

        public string? ImageSpecifications { get; set; }

        [NotMapped]
        public IFormFile? NameImageSpecifications { get; set; }
        public virtual Category? IdCategoryNavigation { get; set; }


    }
}
