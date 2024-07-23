using Ecommerce.Models;
using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public class UserRoleService
    {
        private readonly EcommerceContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRoleService(EcommerceContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            // Lấy RoleId từ bảng AspNetUserRoles
            var roleIds = await _dbContext.AspNetUserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            // Lấy tên quyền từ bảng AspNetRoles
            var roles = await _dbContext.AspNetRoles
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Name)
                .ToListAsync();

            return roles;
        }

        public async Task<List<string>> GetPermissionsForUserAsync(string userId)
        {
            // Lấy RoleId từ bảng AspNetUserRoles
            var roleIds = await _dbContext.AspNetUserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            // Lấy permissions_id từ bảng Role_Permissions
            var permissionIds = await _dbContext.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(rp => rp.PermissionsId)
                .ToListAsync();

            // Lấy tên quyền từ bảng Permissions
            var permissionNames = await _dbContext.AspNetRoles
                .Where(p => permissionIds.Contains(p.Id))
                .Select(p => p.Name)
                .ToListAsync();

            return permissionNames;
        }

        public async Task PrintUserRolesAndPermissionsAsync(string userId)
        {
            var roles = await GetUserRolesAsync(userId);
            var permissions = await GetPermissionsForUserAsync(userId);

            Console.WriteLine($"User ID: {userId}");
            Console.WriteLine("Roles:");
            roles.ForEach(role => Console.WriteLine(role));
            Console.WriteLine("Permissions:");
            permissions.ForEach(permission => Console.WriteLine(permission));
        }

        public async Task<bool> UserHasPermissionAsync(string userId, string permissionName)
        {
            var permissions = await GetPermissionsForUserAsync(userId);
            return permissions.Contains(permissionName);
        }
    }
}




//using Ecommerce.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Ecommerce.Services
//{
//    public class UserRoleService
//    {
//        private readonly EcommerceContext _dbContext;

//        public UserRoleService(EcommerceContext dbContext)
//        {
//            _dbContext = dbContext;
//        }

//        public async Task<List<string>> GetPermissionsForUserAsync(string userId)
//        {
//            var roleIds = await _dbContext.AspNetUserRoles
//                .Where(ur => ur.UserId == userId)
//                .Select(ur => ur.RoleId)
//                .ToListAsync();

//            var permissions = await _dbContext.RolePermissions
//                .Where(rp => roleIds.Contains(rp.RoleId))
//                .Select(rp => rp.PermissionsId)
//                .ToListAsync();

//            // Giả sử bạn có bảng Permissions chứa tên các quyền
//            var permissionNames = await _dbContext.AspNetRoles
//                .Where(p => permissions.Contains(p.Id))
//                .Select(p => p.Name)
//                .ToListAsync();

//            return permissionNames;
//        }
//    }
//}
