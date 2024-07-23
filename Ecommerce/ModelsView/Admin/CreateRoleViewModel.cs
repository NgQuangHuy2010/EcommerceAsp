using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ModelsView.Admin
{
    public class CreateRoleViewModel
    {
        [Required]
        public string? NameRole1 { get; set; }
        [Required(ErrorMessage = "Phải chọn ít nhất một quyền cho vai trò")]
        public List<string> SelectedPermissions { get; set; } = new List<string>();
    }
}
