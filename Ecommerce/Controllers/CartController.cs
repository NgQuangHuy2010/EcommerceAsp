using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ecommerce.Controllers;

public class CartController : Controller
{


    [HttpPost]
    [Route("Cart/AddToCart")]
    public IActionResult AddToCart(int productId, string productName, decimal productPrice, int quantity)
    {
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
                Quantity = quantity
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
        var cart = HttpContext.Session.GetString("Cart");
        if (cart != null)
        {
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            var itemToRemove = cartItems.FirstOrDefault(i => i.Id == productId);
            if (itemToRemove != null)
            {
                decimal removedItemTotalPrice = itemToRemove.TotalPrice;

                cartItems.Remove(itemToRemove);
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));

                // Calculate new total amount after removing item
                decimal totalAmount = cartItems.Sum(i => i.TotalPrice);

                return Json(new { success = true, totalAmount, removedItemTotalPrice });
            }
        }
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
