using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ModelsView.User
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }
}
