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
        }

        HttpContext.Session.SetObject("Cart", cart);
        return Json(new { success = true, message = "Product added to cart." });
    }



    [HttpGet]
    public IActionResult GetCartItemCount()
    {
        var cart = HttpContext.Session.GetString("Cart");
        if (cart != null)
        {
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            var itemCount = cartItems.Sum(item => item.Quantity);
            return Json(new { itemCount = itemCount });
        }
        return Json(new { itemCount = 0 });
    }

    [HttpGet]
    public IActionResult GetTotalPrice()
    {
        var cart = HttpContext.Session.GetString("Cart");
        if (cart != null)
        {
            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cart);
            var totalPrice = cartItems.Sum(item => item.TotalPrice);
            return Json(new { totalPrice = totalPrice });
        }
        return Json(new { totalPrice = 0 });
    }



    [Route("cart")]
    public IActionResult Cart()
    {
        List<CartItem> cart = HttpContext.Session.GetObject<List<CartItem>>("Cart") ?? new List<CartItem>();

        return View(cart);
    }





}
