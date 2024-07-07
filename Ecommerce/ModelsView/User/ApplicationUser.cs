using Microsoft.AspNetCore.Identity;

namespace Ecommerce.ModelsView.User
{
    public class ApplicationUser : IdentityUser
    {
        public string Fullname { get; set; }
        public string Phone { get; set; }
    }
}
