//using Ecommerce.Models;
//using Microsoft.AspNetCore.Mvc;
//namespace Ecommerce.Areas.System.Controllers
//{

//    [Area("system")]
//    [Route("system/account")]
//    public class AccountController : Controller
//    {
//        public EcommerceContext db = new EcommerceContext(); //goi db de ket noi models
//        [Route("index")]
//        //public IActionResult Index()
//        //{

//        //    var list = db.Accounts.ToList();
//        //    return View(list);
//        //}
//        //[Route("add")]
//        //public IActionResult add()
//        //{
//        //    return View();
//        //}
//        //[Route("add")]
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public IActionResult add(Account data)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        db.Accounts.Add(data);
//        //        db.SaveChanges();
//        //        return Redirect("index");
//        //    }
//        //    return View();
//        //}
//        //[Route("delete")]
//        //public IActionResult delete(int? id)
//        //{
//        //    var list = db.Accounts.Find(id);
//        //    if (list != null)
//        //    {
//        //        db.Accounts.Remove(list);
//        //        db.SaveChanges();
//        //    }
//        //    return RedirectToAction("index");
//        //}
//        //[Route("edit")]
//        //[HttpPost]
//        //[ValidateAntiForgeryToken]
//        //public IActionResult edit(int? id, Account data)
//        //{
//        //    if (id == data.Id)
//        //    {
//        //        if (ModelState.IsValid)
//        //        {
//        //            db.Accounts.Update(data);
//        //            db.SaveChanges();
//        //            return RedirectToAction("Index");
//        //        }
//        //        return View(data);
//        //    }
//        //    else
//        //    {
//        //        return RedirectToAction("Index");
//        //    }

//        //}
//        //[Route("edit")]
//        //public IActionResult edit(int? id)
//        //{
//        //    if (id == null)
//        //    {
//        //        return RedirectToAction("index");
//        //    }
//        //    var list = db.Accounts.Find(id);
//        //    if (list == null)
//        //    {
//        //        return RedirectToAction("index");
//        //    }
//        //    return View(list);
//        //}
//    }

//}
