using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
                //trả về json với số tiền đã update
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





}
