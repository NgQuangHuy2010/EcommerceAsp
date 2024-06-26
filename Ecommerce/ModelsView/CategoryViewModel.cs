using Ecommerce.Attributes;
using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.ModelsView
{
    //dùng IViewModelWithId từ Attributes\RequiredIfAttribute.cs để có thể đáp ứng yêu cầu của các phương thức hoặc lớp chung sử dụng thuộc tính Id
    //Hỗ trợ cho các phương pháp chung, chẳng hạn như hàm kiểm tra tồn tại, cập nhật, xóa và các phương thức CRUD khác, mà không cần phải cài đặt lại từng view model riêng lẻ
    public class CategoryViewModel : IViewModelWithId 
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục.")]
        public string? Name { get; set; }

        public string? Image { get; set; }

        [NotMapped]
        //RequiredIfNew sẽ kiểm tra xem khi Id của CategoryViewModel có giá trị là 0 (dạng như tạo mới), nếu tạo mới thì bắt required còn edit thì ko required
        // đc định nghĩa trong Attributes\RequiredIfAttribute.cs
        [RequiredIfNew(nameof(Id), ErrorMessage = "Vui lòng chọn hình ảnh cho danh mục")]
        public IFormFile? NameImage { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    }
}
