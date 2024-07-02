using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Mvc;


using Newtonsoft.Json;

namespace Ecommerce.Controllers;

public class CartController : Controller
{
    [Route("cart")]
    public IActionResult Cart()
    {
        List<CartItem> cart = HttpContext.Session.GetString("Cart") != null
                ? JsonConvert.DeserializeObject<List<CartItem>>(HttpContext.Session.GetString("Cart"))
                : new List<CartItem>();

        return View(cart);
    }

  
}
