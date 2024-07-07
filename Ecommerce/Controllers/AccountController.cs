using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Identity;
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

        //[Route("account")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Login(LoginUserViewModel login)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await db.Accounts.FirstOrDefaultAsync(a => a.Email == login.Email);
        //        if (user != null && BCrypt.Net.BCrypt.Verify(login.Password, user.Password))
        //        {
        //            HttpContext.Session.SetString("IsLoggedIn", "true");
        //            HttpContext.Session.SetString("Id", user.Id.ToString());
        //            HttpContext.Session.SetString("Fullname", user.Fullname);
        //            return RedirectToAction("Index", "Home");
        //        }
        //        ModelState.AddModelError("Email", "Email hoặc mật khẩu không đúng!");
        //    }
        //    return View(login);
        //}

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
                var user = new ApplicationUser
                {
                    UserName = register.Email,
                    Email = register.Email,
                    Fullname = register.Fullname, // Gán giá trị Fullname từ RegisterUserViewModel
                    Phone = register.Phone        // Gán giá trị Phone từ RegisterUserViewModel
                };

                var result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Nếu ModelState.IsValid không thành công, bạn có thể debug ở đây để xem lỗi
            foreach (var state in ModelState)
            {
                if (state.Value.Errors.Any())
                {
                    foreach (var error in state.Value.Errors)
                    {
                        // Log or debug error messages
                        Console.WriteLine($"Error in {state.Key}: {error.ErrorMessage}");
                    }
                }
            }

            return View(register);
        }






    }
}
