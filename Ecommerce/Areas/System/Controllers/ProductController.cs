using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            // Kiểm tra loại tệp trước khi kiểm tra ModelState
            if (product.NameImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(product.NameImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NameImage", "Chỉ chấp nhận các tệp có đuôi: " + string.Join(", ", allowedExtensions));
                }
            }
            if (ModelState.IsValid)
            {
                //Sử dụng FirstOrDefault để tìm một danh mục với Id bằng với IdCategory của sản phẩm.
                var category = db.Category.FirstOrDefault(c => c.Id == product.IdCategory);
                if (category == null)
                {
                    ModelState.AddModelError("IdCategory", "Vui lòng chọn danh mục cho sản phẩm !!");
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


        [Route("edit")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var product = db.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index");
            }

            // Lấy danh sách danh mục
            var categories = db.Category.ToList();
            //tạo một đối tượng SelectList có categories là để tạo danh sách chọn (dropdown list) và id , name là thuộc tính của đối tượng
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.IdCategory);  // product.IdCategory được sử dụng để đánh dấu mục nào trong danh sách được chọn mặc định (selected attribute)

            return View(product);
        }




        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Product products)
        {
            // Kiểm tra loại tệp trước khi kiểm tra ModelState
            if (products.NameImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(products.NameImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NameImage", "Chỉ chấp nhận các tệp có đuôi: " + string.Join(", ", allowedExtensions));
                }
            }

            if (ModelState.IsValid)
            {
                //var load=db.Products.Find(id);
                var load = db.Products.Where(x => x.Id == id).SingleOrDefault();
                if (load != null)
                {
                    if (products.NameImage != null)  //chọn hình
                    {
                        if (load.ImageProduct != null)
                        {
                            string filepath = Path.Combine(environment.WebRootPath, "imgs/imgProducts", load.ImageProduct);
                            if (_fileSystem.File.Exists(filepath))
                            {
                                _fileSystem.File.Delete(filepath);
                            }
                        }
                        string tenhinh = UploadImage(products);
                        load.ImageProduct = tenhinh;

                    }
                    else  //không chọn hình
                    {
                        load.ImageProduct = load.ImageProduct;
                    }
                    load.Description = products.Description;
                    load.NameProduct = products.NameProduct;
                    load.PriceProduct = products.PriceProduct;
                    load.IdCategory = products.IdCategory;
                    load.Discount = products.Discount;
                    load.Model = products.Model;
                    load.Producer = products.Producer;
                    load.Origin = products.Origin;
                    load.Status = products.Status;
                    db.Products.Update(load);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            // Lấy danh sách danh mục
            var categories = db.Category.ToList();
            //tạo một đối tượng SelectList có categories là để tạo danh sách chọn (dropdown list) và id , name là thuộc tính của đối tượng
            ViewBag.Categories = new SelectList(categories, "Id", "Name", products.IdCategory);  // product.IdCategory được sử dụng để đánh dấu mục nào trong danh sách được chọn mặc định (selected attribute)
            return View(products);
        }


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