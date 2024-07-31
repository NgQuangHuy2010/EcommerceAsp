using Ecommerce.Authorization;
using Ecommerce.Models;
using Ecommerce.ModelsView.User;
using Ecommerce.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Sử dụng một chuỗi kết nối duy nhất cho cả EcommerceContext và ApplicationDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EcommerceContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));


// Đăng ký Identity
// Thêm vào dịch vụ Identity với cấu hình mặc định cho ApplicationUser (model user) vào IdentityRole (model Role - vai trò)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = false;  // Không bắt buộc phải có số
    options.Password.RequireLowercase = false;  //// Không bắt buộc phải có chữ thường
    options.Password.RequireNonAlphanumeric = false;  // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false;  // Không bắt buộc chữ in
    options.Password.RequiredLength = 3;   // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1;  // Số ký tự riêng biệt
    //// Cấu hình Lockout - khóa user
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    //options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    //options.Lockout.AllowedForNewUsers = true;
})
    // Thêm triển khai EF lưu trữ thông tin về Idetity (theo AppDbContext -> MS SQL Server).
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // Thêm Token Provider - nó sử dụng để phát sinh token (reset password, confirm email ...)
    // đổi email, số điện thoại ...
    .AddDefaultTokenProviders();



//Cấu hình quyền truy cập
builder.Services.AddAuthorization(options =>
{
    //    // Chính sách cho Admin
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole("Admin");
    });

    // Chính sách cho ProductManager
    options.AddPolicy("RequireProductManagerPermission", policy =>
    {
        policy.Requirements.Add(new PermissionRequirement("ProductManager"));
    });

    // Chính sách cho AccountManager
    options.AddPolicy("RequireAccountManagerPermission", policy =>
    {
        policy.Requirements.Add(new PermissionRequirement("AccountManager"));
    });

    // chặn user truy cập vào System area
    options.AddPolicy("AuthorizeSystemAreas", policy =>
    {
        policy.RequireAssertion(context =>
            !context.User.IsInRole("User") // Người dùng thường không được phép
            || context.User.IsInRole("Admin") // Admin vẫn được phép
            || context.User.HasClaim(c => c.Type == "ProductManager" && c.Value == "True") // ProductManager vẫn được phép
            || context.User.HasClaim(c => c.Type == "AccountManager" && c.Value == "True") // AccountManager vẫn được phép
        );
    });
});



//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("RequireAdminRole", policy =>
//    {
//        policy.RequireRole("Admin");
//    });

//    options.AddPolicy("RequireProductManagerPermission", policy =>
//    {
//        policy.RequireRole("ProductManager");
//    });

//    options.AddPolicy("RequireAccountManagerPermission", policy =>
//    {
//        policy.RequireRole("AccountManager");
//    });

//    // Chính sách tổng hợp (Admin hoặc ProductManager hoặc AccountManager)
//    options.AddPolicy("AdminOrProductManagerOrAccountManager", policy =>
//    {
//        policy.RequireRole("Admin", "ProductManager", "AccountManager");
//    });
//});

// Cấu hình cookie
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";  //Đường dẫn mà người dùng sẽ được chuyển hướng đến khi họ cần phải đăng nhập, để truy cập vào một phần của ứng dụng yêu cầu xác thực(nếu chưa login)
    options.AccessDeniedPath = "/"; // Chuyển hướng về trang chủ của User khi bị từ chối quyền truy cập
});


//đăng ký login google
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//    //options.DefaultScheme = GoogleDefaults.AuthenticationScheme;
//    //options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
//{
//    //clientId và ClientSecret  dc cấu hình ở appsettings.json
//    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
//    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
//});



// Add services to the container.
builder.Services.AddControllersWithViews();


// Đăng ký các dịch vụ khác
builder.Services.AddScoped<UserRoleService, UserRoleService>();

builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
builder.Services.AddTransient<IEmail, Email>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();
// Tạo vai trò và tài khoản admin mặc định khi khởi động ứng dụng
//CreateRolesAndAdminUser có nhiệm vụ tạo vai trò "Admin" và "User" nếu chúng chưa tồn tại trong hệ thống
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await CreateRolesAndAdminUser(roleManager, userManager);
}
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Cấu hình route
//route mặc định của area khi ng dùng ko có quyền admin
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "product",
    pattern: "product/{id}",
    defaults: new { controller = "Products", action = "Products" });
app.Run();
// Tạo vai trò và tài khoản admin mặc định
//CreateRolesAndAdminUser có nhiệm vụ tạo vai trò "Admin" và "User" nếu chúng chưa tồn tại trong hệ thống
async Task CreateRolesAndAdminUser(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    string[] roleNames = { "Admin", "User", "ProductManager", "AccountManager" };  //có thể thêm quyền và chạy https để update
    IdentityResult roleResult;

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Tạo một tài khoản admin mặc định
    var admin = new ApplicationUser
    {
        UserName = "huy2010@gmail.com",
        Email = "huy2010@gmail.com",
        Fullname = "Quang Huy",
        Phone = "0123456789"
    };

    string adminPassword = "201000";
    var _admin = await userManager.FindByEmailAsync("huy2010@gmail.com");

    if (_admin == null)
    {
        var createAdmin = await userManager.CreateAsync(admin, adminPassword);
        if (createAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
