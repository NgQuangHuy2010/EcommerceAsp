using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO.Abstractions;

namespace Ecommerce.Areas.System.Controllers
{

    [Area("system")]
    [Route("system/product")]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly IFileSystem _fileSystem;
        private readonly EcommerceContext db;
        public ProductController(EcommerceContext context, IWebHostEnvironment environment, IFileSystem fileSystem)
        {
            db = context;
            this.environment = environment;
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }
        //ShopContext db = new ShopContext();

        //[Route("index")]
        //public IActionResult Index()
        //{
        //    var list = db..ToList();
        //    return View(list);
        //}
        //[Route("add")]
        //public IActionResult add()
        //{
        //    return View();
        //}

        //[Route("add")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult add(Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var data = new Product
        //        {
        //            Name = product.Name,
        //            Image = UploadImage(product),
        //            Des = product.Des,
        //            Price = product.Price,
        //            Date = product.Date,
        //            IdCate = product.IdCate,
        //            Status = product.Status,
        //        };
        //        db.Products.Add(data);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(product);

        //}


        //public string UploadImage(Product pro)
        //{
        //    string name_image = string.Empty;
        //    if (pro.NameImage != null)
        //    {
        //        String uploadFolder = Path.Combine(environment.WebRootPath, "imgs/");
        //        // String uploadFolder = Path.Combine(environment.WebRootPath,"imgs/");
        //        name_image = DateTime.Now.ToString("dd-MM-yyyy") + "_" + pro.NameImage.FileName;
        //        string filePath = Path.Combine(uploadFolder, name_image);
        //        using (var filestream = new FileStream(filePath, FileMode.Create))
        //        {
        //            pro.NameImage.CopyTo(filestream);
        //        }
        //    }
        //    return name_image;
        //}


        //[Route("edit")]
        //public IActionResult edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    var list_pro = db.Products.Find(id);
        //    if (list_pro == null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //    return View(list_pro);
        //}
        //[Route("edit")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Edit(int? id, Product sp)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //var load=db.Products.Find(id);
        //        var load = db.Products.Where(x => x.Id == id).SingleOrDefault();
        //        if (load != null)
        //        {
        //            if (sp.NameImage != null)  //chọn hình
        //            {
        //                if (load.Image != null)
        //                {
        //                    string filepath = Path.Combine(environment.WebRootPath, "imgs", load.Image);
        //                    if (_fileSystem.File.Exists(filepath))
        //                    {
        //                        _fileSystem.File.Delete(filepath);
        //                    }
        //                }
        //                string tenhinh = UploadImage(sp);
        //                load.Image = tenhinh;

        //            }
        //            else  //không chọn hình
        //            {
        //                load.Image = load.Image;
        //            }
        //            load.Des = sp.Des;
        //            load.Name = sp.Name;
        //            load.Price = sp.Price;
        //            load.IdCate = sp.IdCate;
        //            load.Date = sp.Date;
        //            load.Status = sp.Status;
        //            db.Products.Update(load);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            return RedirectToAction("Index");
        //        }
        //    }
        //    return View(sp);
        //}


        //[Route("delete")]
        //public IActionResult delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return RedirectToAction("index");
        //    }
        //    var list = db.Products.Where(x => x.Id == id).SingleOrDefault();
        //    if (list != null)
        //    {
        //        string folder_image = Path.Combine(environment.WebRootPath, "imgs/");
        //        string hinh = Path.Combine(Directory.GetCurrentDirectory(), folder_image, list.Image);
        //        if (hinh != null)
        //        {
        //            if (_fileSystem.File.Exists(hinh))
        //            {
        //                // Delete the file
        //                _fileSystem.File.Delete(hinh);

        //            }
        //        }
        //        db.Products.Remove(list);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}