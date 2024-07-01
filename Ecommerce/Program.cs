using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("EcommerceContext");
builder.Services.AddDbContext<EcommerceContext>(x => x.UseSqlServer(connectionString));
//pack System.io
builder.Services.AddSingleton<System.IO.Abstractions.IFileSystem, System.IO.Abstractions.FileSystem>();
builder.Services.AddTransient<IEmail, Email>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
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
