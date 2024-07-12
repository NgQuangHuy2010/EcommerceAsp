using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.ModelsView.User
{
    public class ApplicationUser : IdentityUser
    {
        public string? Fullname { get; set; }
        public string? Phone { get; set; }
        [NotMapped]
        public IList<string> Roles { get; set; }
    }
}
