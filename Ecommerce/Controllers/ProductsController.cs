using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            //tìm id danh mục trong sản phẩm 
            var categoryID = details.IdCategory;
            // nếu id danh mục của sản phẩm  hiện tại bằng với id danh mục của sản phẩm khác thì load ra , trừ id hiện tại
            var relatedProducts = db.Products.Where(p => p.IdCategory == categoryID && p.Id != id).ToList();
            var viewModel = new UserProductViewModel
            {
                Details = details,
                RelatedProducts = relatedProducts
            };



            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string searchProduct, int id)
        {
            var category = db.Category.Find(id);
            //sử dụng Entity Framework Core để thực hiện truy vấn bất đồng bộ để lấy toàn bộ danh sách sản phẩm từ cơ sở dữ liệu
            //ToListAsync() chuyển đổi một truy vấn LINQ thành một danh sách (List) các đối tượng Product
            var products = string.IsNullOrEmpty(searchProduct) ? new List<Product>() : await db.Products
                .Where(p => p.NameProduct.Contains(searchProduct))
                .Where(p => p.Model.Contains(searchProduct))
                .ToListAsync();

            var viewModel = new SearchViewModel
            {
                SearchProduct = searchProduct,
                SearchResults = products,
                Categories = db.Category.ToList(),
            };

            return View(viewModel);
        }






    }
}
