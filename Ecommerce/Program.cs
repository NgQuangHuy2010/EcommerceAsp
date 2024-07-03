using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("EcommerceContext");
builder.Services.AddDbContext<EcommerceContext>(x => x.UseSqlServer(connectionString));
//pack System.io
// đăng ký dịch vụ IHttpContextAccessor dùng để add cart get count cart
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
builder.Services.AddTransient<IEmail, Email>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    //thời gian xóa session là 30 phút
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

app.UseAuthorization();
app.UseSession(); // sử dụng session

//cấu hình route product khi nhấn vào danh mục từ trang home
app.MapControllerRoute(
    name: "product",
    pattern: "product/{id}",
    defaults: new { controller = "Products", action = "Products" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
    );

app.Run();
