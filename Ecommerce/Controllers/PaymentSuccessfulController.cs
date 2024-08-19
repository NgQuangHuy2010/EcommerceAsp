using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce.Controllers
{
    public class PaymentSuccessfulController : Controller
    {
        EcommerceContext db = new EcommerceContext();



        [Route("paymentsuccess")]
        public async Task<IActionResult> Index(string partnerCode, string orderId, long amount, string orderInfo, int resultCode, string message)
        {
            if (resultCode == 0) // Kiểm tra nếu thanh toán thành công
            {
                // Lấy dữ liệu từ session
                var paymentData = HttpContext.Session.GetObject<CheckoutViewModel>("PaymentData");

                if (paymentData == null)
                {
                    return BadRequest("Payment data not found in session.");
                }

                // Chuyển đổi danh sách AddressComponents thành một chuỗi
                string detailedAddress = string.Join(", ", paymentData.AddressComponents);

                string products = string.Join(", ", paymentData.Items.Select(i => i.ProductName + " x" + i.Quantity));
                // Lưu thông tin đơn hàng vào cơ sở dữ liệu
                var order = new Order
                {
                    OrderId = orderId,
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    PartnerCode = partnerCode,
                    OrderInfo = orderInfo,
                    Status = message
                };

                db.Orders.Add(order);

                var orderDetail = new OrderDetail
                {
                    OrderId = orderId,
                    Amount = amount,
                    Fullname = paymentData.FullName,
                    Phone = paymentData.PhoneNumber,
                    Email = paymentData.Email,
                    DetailedAddress = detailedAddress,
                    Products = products,
                    PartnerCode = partnerCode,
                    Address = paymentData.Address // Lưu địa chỉ từ session
                };
                db.OrderDetails.Add(orderDetail);

                await db.SaveChangesAsync();
                HttpContext.Session.Remove("PaymentData");
                return View(); // Hiển thị trang PaymentSuccess
            }
            else
            {
                // Xử lý khi thanh toán thất bại
                ViewBag.Message = message;
                return View("PaymentFailed"); // Hiển thị trang PaymentFailed nếu thanh toán thất bại
            }
        }



    }
}
