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
            var viewModel = new HomeViewModel
            {
                Category = db.Category.Take(12).ToList(),
                Products = db.Products.ToList()
            };

            return View(viewModel);
        }
    }
}
