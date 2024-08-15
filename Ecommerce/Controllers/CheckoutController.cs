
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        [Route("checkout")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
