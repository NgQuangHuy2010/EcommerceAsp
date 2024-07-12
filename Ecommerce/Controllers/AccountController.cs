using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [Route("login")]
        public IActionResult Login()
        {
            //kiểm tra xem user đã đăng nhập chưa nếu có quay về home
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();

            }
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginUserViewModel login)
        {
            if (ModelState.IsValid)
            {
                //lockoutOnFailure: false, Không khóa tài khoản sau nhiều lần đăng nhập thất bại
                //isPersistent: false, Cookie xác thực sẽ là cookie phiên, và người dùng sẽ bị đăng xuất khi đóng trình duyệt
                //Hàm PasswordSignInAsync xác thực người dùng bằng cách kiểm tra mật khẩu
                var result = await _signInManager.PasswordSignInAsync(login.Email, login.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded) // nếu check thành công
                {
                    // tiếp tục tìm email đang đăng nhập 
                    var user = await _userManager.FindByEmailAsync(login.Email);
                    //kiểm tra nếu role là Admin (cấu hình ở program.cs) 
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        // return về trang admin 
                        return RedirectToAction("Index", "Category", new { area = "System" });
                    }
                    else
                    {
                        //nếu ko phải admin về home cho user
                        return RedirectToAction("Index", "Home");
                    }
                }
                //check fail email and pass
                ModelState.AddModelError("Email", "Email hoặc mật khẩu không đúng!");
            }
            return View(login);
        }


        [Route("register")]
        public IActionResult Register()
        {
            //kiểm tra xem user đã đăng nhập chưa nếu có quay về home
            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [Route("register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterUserViewModel register)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem email đã tồn tại hay chưa
                var existingUser = await _userManager.FindByEmailAsync(register.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được sử dụng!");
                    return View(register);  // Trả về view đăng ký với lỗi Email đã tồn tại
                }

                var user = new ApplicationUser
                {
                    UserName = register.Email,
                    Email = register.Email,
                    Fullname = register.Fullname, // Gán giá trị Fullname từ RegisterUserViewModel
                    Phone = register.Phone        // Gán giá trị Phone từ RegisterUserViewModel
                };
                // Đăng ký người dùng mới với CreateAsync
                var result = await _userManager.CreateAsync(user, register.Password);

                if (result.Succeeded)
                {
                    //kiểm tra vai trò có tồn tại ko 
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        //tạo mới vai trò user
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }
                    //nếu ko có vai trò gì thì gán quyền mặc định là user
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Login", "Account");
                }
                // Xử lý các lỗi trả về từ Identity khi đăng ký không thành công
                foreach (var error in result.Errors)
                {
                    if (error.Code == "PasswordTooShort")
                    {
                        ModelState.AddModelError("PasswordTooShort", error.Description);
                    }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }





    }
}
