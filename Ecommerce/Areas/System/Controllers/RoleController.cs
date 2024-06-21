using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
namespace Ecommerce.Areas.System.Controllers
{
    [Area("system")]
    [Route("system/role")]
    public class RoleController : Controller
    {
        public EcommerceContext db = new EcommerceContext(); //goi db de ket noi models

        //[Route("index")]
        //public IActionResult Index()
        //{
        //    return View(db.Roles.ToList());
        //}
        //[Route("add")]
        //public IActionResult add()
        //{

        //    return View();
        //}
        //[Route("add")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult add(Role role)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Roles.Add(role);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(role);
        //}
        //[Route("delete")]
        //public IActionResult delete(int id)
        //{
        //    var del = db.Roles.Find(id);
        //    if (del != null)
        //    {
        //        db.Remove(del);
        //        db.SaveChanges();
        //        return RedirectToAction("index");
        //    }
        //    else
        //    {
        //        return RedirectToAction("index");
        //    }
        //}

        //[Route("edit")]
        //[HttpPost]
        //public IActionResult edit(int? id, Role role)
        //{
        //    if (id == role.Id)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            db.Roles.Update(role);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("index");
        //    }
        //}



        //[Route("edit")]

        //public IActionResult edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("index");
        //    }
        //    var edit = db.Roles.Find(id);
        //    if (edit == null)
        //    {
        //        return RedirectToAction("index");
        //    }
        //    return View(edit);

        //}

    }
}
