using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{

    public class HomeController : Controller
    {
        public EcommerceContext db = new EcommerceContext();
        public IActionResult Index()
        {
            var random = new Random();

            // Tạo view model
            var viewModel = new HomeViewModel
            {
                Category = db.Category.Take(12).ToList(),
                AllProducts = db.Products.AsEnumerable().OrderBy(x => random.Next()).Take(20).ToList(),
                RandomProducts = db.Products.AsEnumerable().OrderBy(x => random.Next()).Take(12).ToList(),

            };

            return View(viewModel);
        }




    }
}
