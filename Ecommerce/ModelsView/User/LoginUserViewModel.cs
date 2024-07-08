using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ModelsView.User
{
    public class LoginUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

    }
}
