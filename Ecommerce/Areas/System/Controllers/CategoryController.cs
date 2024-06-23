using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Abstractions;

namespace Ecommerce.Areas.System.Controllers
{
    [Area("system")]
    [Route("system/category")]
    public class CategoryController : Controller
    {
        private readonly IWebHostEnvironment environment;
        private readonly IFileSystem _fileSystem;
        private readonly EcommerceContext db;
        public CategoryController(EcommerceContext context, IWebHostEnvironment environment, IFileSystem fileSystem)
        {
            db = context;
            this.environment = environment;
            _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        }

        [Route("index")]
        public IActionResult Index()
        {
            return View(db.Category.ToList());

        }
        [Route("add")]
        public IActionResult add_category()
        {

            return View();

        }


        [Route("add")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult add_category(Category cate)
        {
            // Kiểm tra loại tệp trước khi kiểm tra ModelState
            if (cate.NameImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(cate.NameImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NameImage", "Chỉ chấp nhận các tệp có đuôi: " + string.Join(", ", allowedExtensions));
                }
            }

            if (ModelState.IsValid)
            {
                var data = new Category
                {
                    Name = cate.Name,
                    Image = UploadImage(cate),
                };
                db.Category.Add(data);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cate);
        }


        public string UploadImage(Category cate)
        {
            string name_image = string.Empty;
            if (cate.NameImage != null)
            {
                string uploadFolder = Path.Combine(environment.WebRootPath, "imgs/imgCategory");
                // Sử dụng GUID để tạo tên tệp duy nhất cho file img
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + cate.NameImage.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    cate.NameImage.CopyTo(filestream);
                }
                name_image = uniqueFileName; // Lưu tên tệp duy nhất vào cơ sở dữ liệu
            }
            return name_image;
        }






        [Route("edit")]
        public IActionResult edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var list = db.Category.Find(id);
            if (list == null)
            {
                return RedirectToAction("Index");

            }
            return View(list);
        }
        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Category cate)
        {
            // Kiểm tra loại tệp trước khi kiểm tra ModelState
            if (cate.NameImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(cate.NameImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NameImage", "Chỉ chấp nhận các tệp có đuôi: " + string.Join(", ", allowedExtensions));
                }
            }

            if (ModelState.IsValid)
            {
                //var load=db.Products.Find(id);
                var load = db.Category.Where(x => x.Id == id).SingleOrDefault();
                if (load != null)
                {
                    if (cate.NameImage != null)  //chọn hình
                    {
                        if (load.Image != null)
                        {
                            string filepath = Path.Combine(environment.WebRootPath, "imgs/imgCategory", load.Image);
                            if (_fileSystem.File.Exists(filepath))
                            {
                                _fileSystem.File.Delete(filepath);
                            }
                        }
                        string tenhinh = UploadImage(cate);
                        load.Image = tenhinh;

                    }
                    else  //không chọn hình
                    {
                        load.Image = load.Image;
                    }
                    load.Name = cate.Name;

                    db.Category.Update(load);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(cate);
        }




        [Route("delete")]
        public IActionResult delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "Id không hợp lệ.";
                return RedirectToAction("Index");
            }

            var category = db.Category.Include(c => c.Products).FirstOrDefault(c => c.Id == id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra nếu danh mục có chứa sản phẩm thì báo lỗi
            if (category.Products.Any())
            {
                TempData["ErrorMessage"] = "Không thể xóa danh mục vì có sản phẩm liên quan.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Category.Remove(category);
                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa danh mục.";
                return RedirectToAction("Index");
            }

            var list = db.Category.Where(x => x.Id == id).SingleOrDefault();
            if (list != null)
            {
                string folder_image = Path.Combine(environment.WebRootPath, "imgs/imgCategory");
                string hinh = Path.Combine(Directory.GetCurrentDirectory(), folder_image, list.Image);
                if (_fileSystem.File.Exists(hinh))
                {
                    _fileSystem.File.Delete(hinh);
                }

                db.Category.Remove(list);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


    }
}
