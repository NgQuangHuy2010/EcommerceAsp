using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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


        [Route("index")]
        public IActionResult Index()
        {
            var listProduct = db.Products.Include(p => p.IdCategoryNavigation).ToList();
            return View(listProduct);
        }
        [Route("add")]
        public IActionResult add()
        {
            ViewBag.Category = db.Category.Select(c => new { c.Id, c.Name }).ToList();
            return View();
        }

        [Route("add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult add(Product product)
        {
            if (ModelState.IsValid)
            {
                //Sử dụng FirstOrDefault để tìm một danh mục với Id bằng với IdCategory của sản phẩm.
                var category = db.Category.FirstOrDefault(c => c.Id == product.IdCategory);
                if (category == null)
                {
                    ModelState.AddModelError("", "Category not found");
                    //dùng biến ViewBag để chuyển dữ liệu từ controller tới view
                    //"ViewBag.Category" sẽ chứa danh sách các đối tượng với hai thuộc tính Id và Name của các danh mục (Category) từ cơ sở dữ liệu.
                    ViewBag.Category = db.Category.Select(c => new { c.Id, c.Name }).ToList();
                    return View(product);
                }

                var data = new Product
                {
                    IdCategory = product.IdCategory,
                    NameProduct = product.NameProduct,
                    ImageProduct = UploadImage(product),
                    Description = product.Description,
                    PriceProduct = product.PriceProduct,
                    Discount = product.Discount,
                    Model = product.Model,
                    Producer = product.Producer,
                    Origin = product.Origin,
                    Status = product.Status,
                    IdCategoryNavigation = category  // Liên kết category với product
                };

                db.Products.Add(data);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.Category = db.Category.Select(c => new { c.Id, c.Name }).ToList();
            return View(product);

        }


        public string UploadImage(Product pro)
        {
            string name_image = string.Empty;
            if (pro.NameImage != null)
            {
                string uploadFolder = Path.Combine(environment.WebRootPath, "imgs/imgProducts");
                // Sử dụng GUID để tạo tên tệp duy nhất cho file img
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + pro.NameImage.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    pro.NameImage.CopyTo(filestream);
                }
                name_image = uniqueFileName; // Lưu tên tệp duy nhất vào cơ sở dữ liệu
            }
            return name_image;
        }


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
        //                    string filepath = Path.Combine(environment.WebRootPath, "imgs/imgProducts", load.Image);
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


        [Route("delete")]
        public IActionResult delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("index");
            }
            var list = db.Products.Where(x => x.Id == id).SingleOrDefault();
            if (list != null)
            {
                string folder_image = Path.Combine(environment.WebRootPath, "imgs/imgProducts");
                string hinh = Path.Combine(Directory.GetCurrentDirectory(), folder_image, list.ImageProduct);
                if (hinh != null)
                {
                    if (_fileSystem.File.Exists(hinh))
                    {
                        // Delete the file
                        _fileSystem.File.Delete(hinh);

                    }
                }
                db.Products.Remove(list);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}