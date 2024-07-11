using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.System.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("system")]
    [Route("system/account")]
    public class AccountManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountManagementController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [Route("index")]
        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();

            foreach (var user in users)
            {
                user.Roles = await _userManager.GetRolesAsync(user);
            }

            return View(users);
        }
    }
}
