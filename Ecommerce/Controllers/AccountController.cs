using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace Ecommerce.Controllers
{
    // [Route("account")]

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


        public async Task LoginWithGoogle()
        {
            //var redirectUri = Url.Action("GoogleResponse");
            //Console.WriteLine($"RedirectUri: {redirectUri}");
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (result?.Succeeded == true)
            {
                // Lấy thông tin từ Claims
                var claims = result.Principal.Claims.ToList();
                var userId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var fullName = result.Principal.FindFirstValue(ClaimTypes.Name);

                // Lưu thông tin vào cơ sở dữ liệu
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        Fullname = fullName // Thêm thông tin tên đầy đủ nếu cần thiết
                                            // Các thông tin khác có thể lưu tại đây
                    };
                    var resultCreate = await _userManager.CreateAsync(user);
                    if (!resultCreate.Succeeded)
                    {
                        // Xử lý khi không tạo người dùng thành công
                        return RedirectToAction("Index", "Home"); // hoặc trả về một view lỗi
                    }
                }

                // Đăng nhập người dùng vào hệ thống
                await _signInManager.SignInAsync(user, isPersistent: false);

                // Chuyển hướng sau khi đăng nhập thành công
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Xử lý khi không xác thực thành công
                return RedirectToAction("Index", "Home"); // hoặc trả về một view lỗi
            }
        }


        //public async Task<IActionResult> GoogleResponse()
        //{
        //    var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
        //    if (result?.Succeeded == true)
        //    {
        //        var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
        //        {
        //            claim.Issuer,
        //            claim.OriginalIssuer,
        //            claim.Type,
        //            claim.Value
        //        });
        //        Console.WriteLine("Claims:");
        //        foreach (var claim in claims)
        //        {
        //            Console.WriteLine($"{claim.Type}: {claim.Value}");
        //        }

        //        // Xử lý các claims
        //        return RedirectToAction("Index", "Home");
        //        // return Json(claims);
        //    }
        //    else
        //    {
        //        Console.WriteLine("Authentication failed.");
        //        // Xử lý khi không xác thực thành công
        //        return RedirectToAction("Index", "Home"); // hoặc trả về một view lỗi
        //    }
        //}



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Logout()
        //{
        //    await HttpContext.SignOutAsync();
        //    return RedirectToAction("Index", "Home");

        //}

        //khi ko quyền admin trả về action này 
        public IActionResult AccessDenied()
        {
            return RedirectToAction("Index", "Home");
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
