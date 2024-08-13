using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace Ecommerce.Controllers;

public class CartController : Controller
{
    public EcommerceContext db = new EcommerceContext();

    [HttpPost]
    [Route("Cart/AddToCart")]
    public IActionResult AddToCart(int productId, string productName, decimal productPrice, int quantity)
    {
        //lấy id product
        var product = db.Products.FirstOrDefault(p => p.Id == productId);
        List<CartItem> cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        var existingItem = cart.FirstOrDefault(item => item.Id == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            HttpContext.Session.SetObject("Cart", cart);
            return Json(new { success = true, message = "Số lượng sản phẩm đã được cập nhật!", newItem = false });
        }
        else
        {
            cart.Add(new CartItem
            {
                Id = productId,
                ProductName = productName,
                ProductPrice = productPrice,
                Quantity = quantity,
                ImageProductCart = product.ImageProduct // Set image thông qua model product 
            });
            HttpContext.Session.SetObject("Cart", cart);
            return Json(new { success = true, message = "Thêm vào giỏ hàng thành công!", newItem = true });
        }
    }




    [HttpGet]
    [Route("Cart/GetCartItemCount")]
    public IActionResult GetCartItemCount()
    {
        //lấy session ra từ getObject 
        var cart = HttpContext.Session.GetObject<List<CartItem>>("Cart");
        if (cart != null) // nếu không null
        {
            var uniqueItemCount = cart.Count; // Đếm số lượng sản phẩm duy nhất
            return Json(new { itemCount = uniqueItemCount }); //Trả về số lượng sản phẩm duy nhất dưới dạng JSON
        }
        return Json(new { itemCount = 0 }); //Nếu giỏ hàng trống hoặc không tồn tại, trả về itemCount bằng 0
    }


    [HttpPost]
    public IActionResult UpdateCart(int productId, int quantity)
    {
        //Lấy giỏ hàng từ session dưới dạng chuỗi JSON
        var cart = HttpContext.Session.GetString("Cart");
        if (cart != null) // nếu không null
        {
            //chuyển đổi chuỗi Json thành dạng Object
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            // tìm id sản phẩm ,cú pháp LINQ (Language Integrated Query) để tìm đối tượng
            var item = cartItems.FirstOrDefault(i => i.Id == productId);
            if (item != null)
            {
                //update số lượng
                item.Quantity = quantity;
                //Lưu lại giỏ hàng đã cập nhật vào session dưới dạng chuỗi JSON
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
                decimal totalAmount = cartItems.Sum(i => i.TotalPrice);

                Console.WriteLine($"Total Amount: {totalAmount.ToString("N0")} ₫");                //trả về json với số tiền đã update
                return Json(new { success = true, totalPrice = item.TotalPrice.ToString("N0") + " ₫", totalAmount = totalAmount.ToString("N0") + " ₫" });
            }
        }
        return Json(new { success = false, message = "Item not found in cart." });
    }

    [HttpPost]
    public IActionResult RemoveFromCart(int productId)
    {
        // Lấy giỏ hàng từ session dưới dạng chuỗi JSON
        var cart = HttpContext.Session.GetString("Cart");

        // Kiểm tra nếu giỏ hàng không null
        if (cart != null)
        {
            // Chuyển đổi chuỗi JSON thành đối tượng List<CartItem>
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);

            // Tìm sản phẩm trong giỏ hàng bằng productId
            var itemToRemove = cartItems.FirstOrDefault(i => i.Id == productId);

            // Nếu sản phẩm được tìm thấy (ko null)
            if (itemToRemove != null)
            {
                // Lưu lại tổng giá của sản phẩm sẽ bị xóa
                decimal removedItemTotalPrice = itemToRemove.TotalPrice;

                // Xóa sản phẩm khỏi cart
                cartItems.Remove(itemToRemove);

                // Cập nhật lại cart vào session dưới dạng chuỗi JSON
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));

                // load lại tổng số tiền khi có sản phẩm bị xóa
                decimal totalAmount = cartItems.Sum(i => i.TotalPrice);

                // Trả về JSON với các thông tin đã cập nhật, bao gồm tổng số tiền mới và tổng giá của sản phẩm đã bị xóa
                return Json(new { success = true, totalAmount, removedItemTotalPrice });
            }
        }
        // Nếu sản phẩm không được tìm thấy trong giỏ hàng, trả về thông báo lỗi
        return Json(new { success = false, message = "Item not found in cart." });
    }




    [Route("cart")]
    public IActionResult Cart()
    {
        //lấy session từ getObject
        var cartItems = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();
        var totalCart = new TotalCart
        {
            Items = cartItems
        };
        return View(totalCart);
    }

    private static readonly HttpClient client = new HttpClient();

    private async Task<string> ExecPostRequest(string url, string data)
    {
        var content = new StringContent(data, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    [HttpPost]
    public async Task<IActionResult> MomoPayment()
    {
        // Lấy giỏ hàng từ session
        var cart = HttpContext.Session.GetString("Cart");
        if (cart == null)
        {
            return BadRequest("Cart is empty.");
        }

        // Chuyển đổi chuỗi JSON thành danh sách CartItem
        var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
        if (cartItems == null || !cartItems.Any())
        {
            return BadRequest("Cart is empty.");
        }

        // Tính tổng số tiền của giỏ hàng
        decimal totalAmount = cartItems.Sum(i => i.TotalPrice);

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




    public IActionResult Error()
    {
        return View();
    }

}
