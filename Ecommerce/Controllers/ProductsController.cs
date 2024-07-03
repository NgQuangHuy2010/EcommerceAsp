using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
namespace Ecommerce.Controllers
{
    public class ProductsController : Controller
    {
        public EcommerceContext db = new EcommerceContext();

        public IActionResult Products(int id)
        {
            //lấy id của category
            var category = db.Category.Find(id);
            //so sánh id từ url của category có bằng với id khóa ngoại của bảng Product không
            var categoryProducts = db.Products.Where(p => p.IdCategory == id).ToList();

            var viewModel = new UserProductViewModel
            {
                Categories = db.Category.ToList(),
                Products = categoryProducts
            };

            return View(viewModel);
        }
        [Route("product/details/{id}")]
        public IActionResult ProductDetails(int id)
        {
            var details = db.Products.Find(id);
            if (details == null)
            {
                return NotFound();
            }

            var viewModel = new UserProductViewModel
            {
                Details = details
            };

            return View(viewModel);
        }








    }
}
