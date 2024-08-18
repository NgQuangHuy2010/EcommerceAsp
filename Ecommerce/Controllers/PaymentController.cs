using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Controllers
{
    public class PaymentController : Controller
    {
        //thanh toán momo
        private static readonly HttpClient client = new HttpClient();

        private async Task<string> ExecPostRequest(string url, string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            return await response.Content.ReadAsStringAsync();
        }

        [HttpPost]
        public async Task<IActionResult> MomoPayment(CheckoutViewModel model)
        {
            // Kiểm tra và log dữ liệu từ session
            var checkoutViewModel = HttpContext.Session.GetObject<CheckoutViewModel>("CheckoutCart");
            if (checkoutViewModel == null)
            {
                // Log lỗi nếu session bị mất
                Console.WriteLine("CheckoutCart session is null.");
                return RedirectToAction("Index", "Cart");
            }

            // Phân tích các thành phần địa chỉ từ JSON
            if (!string.IsNullOrEmpty(Request.Form["AddressComponents"]))
            {
                var addressComponentsJson = Request.Form["AddressComponents"];
                try
                {
                    model.AddressComponents = JsonConvert.DeserializeObject<List<string>>(addressComponentsJson);
                }
                catch (JsonReaderException ex)
                {
                    // Log lỗi nếu không giải mã được JSON
                    Console.WriteLine($"Lỗi khi giải mã dữ liệu JSON: {ex.Message}");
                    Console.WriteLine($"Dữ liệu JSON: {addressComponentsJson}");
                    // Có thể thêm một thông báo lỗi cho người dùng hoặc hành động khác tại đây
                }
            }

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

            // Xử lý và lưu thông tin vào session
            model.Items = checkoutViewModel.Items;
            model.Shipping = checkoutViewModel.Shipping;
            model.TotalAmount = checkoutViewModel.TotalAmount;
            model.TotalPayment = checkoutViewModel.TotalPayment;

            HttpContext.Session.SetObject("PaymentData", model);
            var paymentData = HttpContext.Session.GetObject<CheckoutViewModel>("PaymentData");
            if (paymentData != null)
            {
                Console.WriteLine("PaymentData session contains data.");
                Console.WriteLine($"Họ và tên: {paymentData.FullName}");
                Console.WriteLine($"Địa chỉ: {paymentData.Address}");
                Console.WriteLine($"Số điện thoại: {paymentData.PhoneNumber}");
                Console.WriteLine($"Email: {paymentData.Email}");
                Console.WriteLine($"Phí vận chuyển: {paymentData.Shipping}");
                Console.WriteLine($"Tổng tiền thanh toán: {paymentData.TotalPayment}");
                Console.WriteLine("Các sản phẩm:");
                if (paymentData.Items != null)
                {
                    foreach (var item in paymentData.Items)
                    {
                        Console.WriteLine($"Tên sản phẩm: {item.ProductName}, Số lượng: {item.Quantity}, Giá: {item.TotalPrice}");
                    }
                }
                if (paymentData.AddressComponents != null)
                {
                    Console.WriteLine($"Địa chỉ chi tiết: {string.Join(", ", paymentData.AddressComponents)}");
                }
            }
            else
            {
                Console.WriteLine("PaymentData session is empty.");
            }

            //// Chuyển đổi chuỗi JSON thành danh sách
            ////var cartItems = JsonConvert.DeserializeObject<CheckoutViewModel>(amountMomo);
            ////if (cartItems == null)
            ////{
            ////    return BadRequest("Cart is empty.");
            ////}

            ////// lấy tổng tiền
            decimal totalAmount = paymentData.TotalPayment;

            // Thiết lập thông tin thanh toán MoMo
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOBKUN20180529";
            string accessKey = "klm05TvNBzhg7h7j";
            string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
            string orderInfo = "Thanh toán qua MoMo";
            string orderId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string redirectUrl = "http://localhost:84/Traveltour_github/payment/confirm";
            string ipnUrl = "http://localhost:84/Traveltour_github/payment/confirm";
            string extraData = "";
            string requestId = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
            string requestType = "payWithATM";

            // Tạo chuỗi rawHash cho chữ ký
            string rawHash = $"accessKey={accessKey}&amount={totalAmount.ToString("F0")}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
            string signature;

            // Tạo chữ ký HMACSHA256
            using (HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawHash));
                signature = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }

            // Tạo dữ liệu yêu cầu thanh toán
            var data = new
            {
                partnerCode,
                partnerName = "Test",
                storeId = "MomoTestStore",
                requestId,
                amount = totalAmount.ToString("F0"),
                orderId,
                orderInfo,
                redirectUrl,
                ipnUrl,
                lang = "vi",
                extraData,
                requestType,
                signature
            };

            // Chuyển đổi dữ liệu thành JSON và gửi yêu cầu POST
            string jsonData = JsonConvert.SerializeObject(data);
            string result = await ExecPostRequest(endpoint, jsonData);
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            // Kiểm tra kết quả và chuyển hướng đến URL thanh toán
            if (jsonResult != null && jsonResult.ContainsKey("payUrl"))
            {
                return Redirect(jsonResult["payUrl"]);
            }

            return BadRequest("Payment request failed.");
        }
    }
}
