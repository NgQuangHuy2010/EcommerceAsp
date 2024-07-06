using Microsoft.AspNetCore.Identity;
using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [Route("account")]
        public IActionResult Login()
        {
            return View();
        }
        [Route("register")]
        public IActionResult Register()
        {
            return View();
        }
        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel register)
        {

            if (ModelState.IsValid)
            {
                
            }
            return View();
        }

    }
}
