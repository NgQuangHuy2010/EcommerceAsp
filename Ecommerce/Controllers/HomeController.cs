using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        public EcommerceContext db = new EcommerceContext();
        public IActionResult Index()
        {
            var listCategory = db.Category.Take(12).ToList();
            return View(listCategory);
        }
    }
}
