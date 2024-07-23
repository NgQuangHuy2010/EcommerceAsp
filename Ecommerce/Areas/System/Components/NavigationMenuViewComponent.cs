using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Ecommerce.Areas.System.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly UserRoleService _userRoleService;

        public NavigationMenuViewComponent(UserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isProductManager = await _userRoleService.UserHasPermissionAsync(userId, "ProductManager");
            var isAccountManager = await _userRoleService.UserHasPermissionAsync(userId, "AccountManager");

            var isAdmin = HttpContext.User.IsInRole("Admin");

            var model = new NavigationMenuViewModel
            {
                IsProductManager = isProductManager,
                IsAccountManager = isAccountManager,
                IsAdmin = isAdmin
            };

            return View(model);
        }
    }

    public class NavigationMenuViewModel
    {
        public bool IsProductManager { get; set; }
        public bool IsAccountManager { get; set; }

        public bool IsAdmin { get; set; }
    }
}
