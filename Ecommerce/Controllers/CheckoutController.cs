
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Ecommerce.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public CheckoutController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [Route("checkout")]
        public IActionResult Index()
        {
            var checkoutViewModel = HttpContext.Session.GetObject<CheckoutViewModel>("CheckoutCart");
            if (checkoutViewModel == null)
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(checkoutViewModel);
        }

        [HttpPost]
        [Route("checkout")]
        public IActionResult Checkout()
        {
            // Lấy giỏ hàng từ session
            List<CartItem> cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Count == 0)
            {
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống!" });
            }
            //tạo đối tượng chứa các thông tin trong giỏ hàng (tái dùng từ cart)
            var totalCart = new TotalCart
            {
                Items = cart,
            };
            var checkoutViewModel = new CheckoutViewModel
            {
                Items = totalCart.Items,
                Shipping = totalCart.Shipping,
                TotalAmount = totalCart.TotalAmount,
                TotalPayment = totalCart.TotalPayment
            };
            // In ra console thông tin tất cả các sản phẩm trong giỏ hàng
            Console.WriteLine("Thông tin giỏ hàng khi bấm thanh toán:");
            foreach (var item in checkoutViewModel.Items)
            {
                Console.WriteLine($"Id: {item.Id}, Tên sản phẩm: {item.ProductName}, Giá: {item.ProductPrice}, Số lượng: {item.Quantity}, Tổng tiền: {item.ProductPrice * item.Quantity}");
            }
            Console.WriteLine($"Phí vận chuyển: {checkoutViewModel.Shipping}");
            Console.WriteLine($"Tổng tiền thanh toán: {checkoutViewModel.TotalPayment}");
            HttpContext.Session.SetObject("CheckoutCart", checkoutViewModel);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetLocations()
        {
            // Xác định đường dẫn đến tệp JSON từ thư mục Data
            // _webHostEnvironment.ContentRootPath cung cấp đường dẫn gốc của ứng dụng
            var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Data", "locations.json");

            // Đọc toàn bộ nội dung của tệp JSON vào một chuỗi
            var json = System.IO.File.ReadAllText(filePath);

            // Chuyển đổi chuỗi JSON thành một đối tượng JArray để dễ thao tác
            var filteredData = JArray.Parse(json).Select(loc => new
            {
                // Lấy tên của tỉnh/thành phố từ thuộc tính "name"
                name = (string)loc["name"],

                // Lấy mã của tỉnh/thành phố từ thuộc tính "code"
                code = (int)loc["code"],

                // Chuyển đổi danh sách các quận/huyện
                districts = loc["districts"].Select(d => new
                {
                    // Lấy tên của quận/huyện từ thuộc tính "name"
                    name = (string)d["name"],

                    // Lấy mã của quận/huyện từ thuộc tính "code"
                    code = (int)d["code"],

                    // Chuyển đổi danh sách các phường/xã của quận/huyện
                    wards = d["wards"].Select(w => new
                    {
                        // Lấy tên của phường/xã từ thuộc tính "name"
                        name = (string)w["name"],

                        // Lấy mã của phường/xã từ thuộc tính "code"
                        code = (int)w["code"]
                    }).ToList() // Chuyển đổi danh sách các phường/xã thành List
                }).ToList()
            }).ToList();
            return Json(filteredData);
        }




    }
}
