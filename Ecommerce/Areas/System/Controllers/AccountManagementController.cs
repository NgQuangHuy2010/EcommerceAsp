using Ecommerce.ModelsView.Admin;
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
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		public AccountManagementController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
		}
		[Route("index")]
		public async Task<IActionResult> Index()
		{
			var users = _userManager.Users.ToList();

			foreach (var user in users)
			{
				user.Roles = await _userManager.GetRolesAsync(user);
			}

			// Chỉ giữ lại những người dùng có vai trò "User"
			var userViewModels = users
				.Where(u => u.Roles.Contains("User") && !u.Roles.Contains("Admin"))
				.ToList();

			return View(userViewModels);
		}

		[Route("create")]
		public IActionResult Create()
		{
			return View();
		}
		[Route("create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CreateAccountAdminViewModel register)
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
					return RedirectToAction(nameof(Index));
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
		[HttpGet]
		[Route("update/{id}")]
		public async Task<IActionResult> Update(string id)
		{
			if (id == null)
			{
				return NotFound();
			}
			//lấy id từ usermanager 
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}
			//get các data từ viewmodel ra giao diện
			var model = new UpdateAccountAdminViewModel
			{
				Id = user.Id,
				Email = user.Email,
				Fullname = user.Fullname,
				Phone = user.Phone
			};

			return View(model);
		}

		[HttpPost]
		[Route("update/{id}")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Update(string id, UpdateAccountAdminViewModel model)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				user.Email = model.Email;
				user.Fullname = model.Fullname;
				user.Phone = model.Phone;
				//kiểm tra xem password của ng dùng có trống ko, Nếu Password không trống (nghĩa là người dùng đã nhập mật khẩu mới)
				if (!string.IsNullOrEmpty(model.Password))
				{
					// Tạo token đặt lại mật khẩu
					var token = await _userManager.GeneratePasswordResetTokenAsync(user);

					// Đặt lại mật khẩu của người dùng bằng cách sử dụng token và mật khẩu mới
					var result = await _userManager.ResetPasswordAsync(user, token, model.Password);

					// Kiểm tra xem việc đặt lại mật khẩu có thành công không
					if (!result.Succeeded)
					{
						// Nếu việc đặt lại mật khẩu không thành công, thêm các lỗi vào ModelState để hiển thị trên giao diện
						foreach (var error in result.Errors)
						{
							ModelState.AddModelError(string.Empty, error.Description);
						}

						// Trả về lại view với model hiện tại để hiển thị các lỗi
						return View(model);
					}
				}
				// update data mới
				var updateResult = await _userManager.UpdateAsync(user);
				if (updateResult.Succeeded)
				{
					return RedirectToAction("Index");
				}

				foreach (var error in updateResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View(model);
		}






		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			var result = await _userManager.DeleteAsync(user);
			if (result.Succeeded)
			{
				return RedirectToAction(nameof(Index));
			}

			// Nếu có lỗi, thêm thông báo lỗi vào ModelState
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View("Index", _userManager.Users.ToList());
		}


	}
}
