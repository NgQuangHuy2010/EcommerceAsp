using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        [Route("product")]
        public IActionResult Products()
        {
            return View();
        }
    }
}
