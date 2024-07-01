using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Ecommerce.Controllers.ProductsController;
using Ecommerce.ModelsView;
namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        [Route("cart")]
        public IActionResult Cart()
        {
            var cart = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrEmpty(cart)
                            ? new List<CartItem>()
                            : JsonConvert.DeserializeObject<List<CartItem>>(cart);

            return View(cartItems);
        }

        public IActionResult Remove(int productId)
        {
            var cart = HttpContext.Session.GetString("Cart");
            var cartItems = string.IsNullOrEmpty(cart)
                            ? new List<CartItem>()
                            : JsonConvert.DeserializeObject<List<CartItem>>(cart);

            var item = cartItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null)
            {
                cartItems.Remove(item);
                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartItems));
            }

            return RedirectToAction("Cart");
        }
    }
}
