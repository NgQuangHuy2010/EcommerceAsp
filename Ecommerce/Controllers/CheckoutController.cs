
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
        public async Task<IActionResult> Index(CheckoutViewModel model)
        {
            //// Kiểm tra và log dữ liệu từ session
            var checkoutViewModel = HttpContext.Session.GetObject<CheckoutViewModel>("CheckoutCart");
            //if (checkoutViewModel == null)
            //{
            //    // Log lỗi nếu session bị mất
            //    Console.WriteLine("CheckoutCart session is null.");
            //    return RedirectToAction("Index", "Cart");
            //}

            //// Phân tích các thành phần địa chỉ từ JSON
            //if (!string.IsNullOrEmpty(Request.Form["AddressComponents"]))
            //{
            //    var addressComponentsJson = Request.Form["AddressComponents"];
            //    try
            //    {
            //        model.AddressComponents = JsonConvert.DeserializeObject<List<string>>(addressComponentsJson);
            //    }
            //    catch (JsonReaderException ex)
            //    {
            //        // Log lỗi nếu không giải mã được JSON
            //        Console.WriteLine($"Lỗi khi giải mã dữ liệu JSON: {ex.Message}");
            //        Console.WriteLine($"Dữ liệu JSON: {addressComponentsJson}");
            //        // Có thể thêm một thông báo lỗi cho người dùng hoặc hành động khác tại đây
            //    }
            //}

            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                // Log lỗi nếu model không hợp lệ
                Console.WriteLine("Model state is not valid.");
                // Trả lại view với lỗi và dữ liệu session
                model.Items = checkoutViewModel.Items;
                model.Shipping = checkoutViewModel.Shipping;
                model.TotalAmount = checkoutViewModel.TotalAmount;
                model.TotalPayment = checkoutViewModel.TotalPayment;
                return View(model);
            }

            //// Xử lý và lưu thông tin vào session
            //model.Items = checkoutViewModel.Items;
            //model.Shipping = checkoutViewModel.Shipping;
            //model.TotalAmount = checkoutViewModel.TotalAmount;
            //model.TotalPayment = checkoutViewModel.TotalPayment;

            //HttpContext.Session.SetObject("PaymentData", model);
            //var paymentData = HttpContext.Session.GetObject<CheckoutViewModel>("PaymentData");
            //if (paymentData != null)
            //{
            //    Console.WriteLine("PaymentData session contains data.");
            //    Console.WriteLine($"Họ và tên: {paymentData.FullName}");
            //    Console.WriteLine($"Địa chỉ: {paymentData.Address}");
            //    Console.WriteLine($"Số điện thoại: {paymentData.PhoneNumber}");
            //    Console.WriteLine($"Email: {paymentData.Email}");
            //    Console.WriteLine($"Phí vận chuyển: {paymentData.Shipping}");
            //    Console.WriteLine($"Tổng tiền thanh toán: {paymentData.TotalPayment}");
            //    Console.WriteLine("Các sản phẩm:");
            //    if (paymentData.Items != null)
            //    {
            //        foreach (var item in paymentData.Items)
            //        {
            //            Console.WriteLine($"Tên sản phẩm: {item.ProductName}, Số lượng: {item.Quantity}, Giá: {item.TotalPrice}");
            //        }
            //    }
            //    if (paymentData.AddressComponents != null)
            //    {
            //        Console.WriteLine($"Địa chỉ chi tiết: {string.Join(", ", paymentData.AddressComponents)}");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("PaymentData session is empty.");
            //}



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
