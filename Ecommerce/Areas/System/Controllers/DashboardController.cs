using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Areas.System.Controllers
{
    [Area("system")]
    [Route("system")]
    public class DashboardController : Controller
    {
        [Route("dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
