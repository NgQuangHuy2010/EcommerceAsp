using Ecommerce.Models;
using Ecommerce.ModelsView.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Sử dụng một chuỗi kết nối duy nhất cho cả EcommerceContext và ApplicationDbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<EcommerceContext>(x => x.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Đăng ký Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false; // Không yêu cầu phải có chữ số trong mật khẩu.
    options.Password.RequireLowercase = false; //Không yêu cầu phải có chữ cái thường trong mật khẩu
    options.Password.RequireNonAlphanumeric = false; //hông yêu cầu phải có ký tự đặc biệt trong mật khẩu
    options.Password.RequireUppercase = false; //Không yêu cầu phải có chữ cái hoa trong mật khẩu
    options.Password.RequiredLength = 3; //Yêu cầu độ dài tối thiểu của mật khẩu là 3 ký tự.
    options.Password.RequiredUniqueChars = 1; // Số ký tự khác nhau yêu cầu trong mật khẩu
})
.AddEntityFrameworkStores<ApplicationDbContext>() // Sử dụng EF Core làm storage cho Identity
.AddDefaultTokenProviders();

// Đăng ký các dịch vụ khác
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
builder.Services.AddTransient<IEmail, Email>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    // thời gian xóa session là 30 phút
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
// Authentication và Authorization
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Sử dụng session
// Cấu hình route
app.MapControllerRoute(
    name: "product",
    pattern: "product/{id}",
    defaults: new { controller = "Products", action = "Products" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
