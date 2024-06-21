//using Ecommerce.Models;
//using Microsoft.AspNetCore.Mvc;

//namespace Ecommerce.Controllers
//{
//    public class SecureController : Controller
//    {
//        private ShopContext db;
//        private readonly IEmail _email;
//        public SecureController(ShopContext context, IEmail email)
//        {
//            db = context;
//            _email = email;
//        }
//        [Route("login")]
//        public IActionResult login()
//        {
//            return View();
//        }

//        [Route("login")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult login(Account account)
//        {
//            if (ModelState.IsValid)
//            {
//                var login = db.Accounts.Where(a => a.Email == account.Email && a.Password == account.Password).SingleOrDefault();
//                if (login == null)
//                {
//                    ModelState.AddModelError("loi", "Thong tin ko dung");
//                }
//                else
//                {
//                    return RedirectToAction("Index", "Account", new { area = "System" });
//                }
//            }
//            return View();
//        }

//        [Route("register")]
//        public IActionResult register()
//        {

//            return View();
//        }
//        [Route("register")]
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult register(Account account)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Accounts.Add(account);
//                db.SaveChanges();
//                return RedirectToAction("login");
//            }
//            return View();
//        }
//        [Route("Mail")]
//        public async Task<IActionResult> mail()
//        {
//            string to = "nqht123456789@gmail.com";
//            string subject = "laskfnsnf";
//            string body = "test asp";
//            await _email.SendEmailAsync(to, subject, body);
//            return View();
//        }
//    }
//}
