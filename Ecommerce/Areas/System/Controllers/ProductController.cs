using Ecommerce.Models;
using Ecommerce.ModelsView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO.Abstractions;
namespace Ecommerce.Areas.System.Controllers
{

    [Authorize(Policy = "AuthorizeSystemAreas")]
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
        public IActionResult add(ProductsViewModel product)
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

            if (product.NameImageSpecifications != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(product.NameImageSpecifications.FileName).ToLower();

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
                    ImageSpecifications = UploadImageSpecifications(product),
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


        public string UploadImageSpecifications(ProductsViewModel pro)
        {
            string name_image_specifications = string.Empty;
            if (pro.NameImageSpecifications != null)
            {
                string uploadFolder = Path.Combine(environment.WebRootPath, "imgs/imgProducts");
                // Sử dụng GUID để tạo tên tệp duy nhất cho file img
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + pro.NameImageSpecifications.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var filestream = new FileStream(filePath, FileMode.Create))
                {
                    pro.NameImageSpecifications.CopyTo(filestream);
                }
                name_image_specifications = uniqueFileName; // Lưu tên tệp duy nhất vào cơ sở dữ liệu
            }
            return name_image_specifications;
        }
        public string UploadImage(ProductsViewModel pro)
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

            // Ánh xạ từ Product sang ProductsViewModel để get các giá trị ra view 
            var productsViewModel = new ProductsViewModel
            {
                Id = product.Id,
                IdCategory = product.IdCategory,
                NameProduct = product.NameProduct,
                Description = product.Description,
                PriceProduct = product.PriceProduct,
                Discount = product.Discount,
                Model = product.Model,
                Producer = product.Producer,
                Origin = product.Origin,
                Status = product.Status,
                ImageProduct = product.ImageProduct,
                ImageSpecifications = product.ImageSpecifications,
            };

            // Lấy danh sách danh mục trong category bằng seleclist để hiện ra view bằng viewbag
            var categories = db.Category.ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", product.IdCategory);
            //lấy đường dẫn hình ảnh để hiện hình ảnh ra view để biết hình ảnh hiện tại là hình gì
            ViewBag.ImagePath = GetImagePath(product.ImageProduct);
            ViewBag.ImagePathSpecifications = GetImagePath(product.ImageSpecifications);
            // Trả về view Edit với ProductsViewModel
            return View(productsViewModel);
        }

        private string GetImagePath(string imageFileName)
        {
            if (string.IsNullOrEmpty(imageFileName))
            {
                return "Không có hình ảnh";
            }

            // Đường dẫn đầy đủ đến thư mục chứa hình ảnh
            var imagePath = Path.Combine("/imgs/imgProducts", imageFileName);
            return imagePath;
        }



        [Route("edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, ProductsViewModel products)
        {
            //tìm id của category và đưa đường dẫn vào viewbag để luôn hiện hình ảnh của id đó thông qua GetImagePath 
            var product = db.Products.Find(id);
            ViewBag.ImagePath = GetImagePath(product.ImageProduct);
            ViewBag.ImagePathSpecifications = GetImagePath(product.ImageSpecifications);


            //kiểm tra xem đúng định dạng file ảnh không
            if (products.NameImage != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(products.NameImage.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NameImage", "Chỉ chấp nhận các tệp có đuôi: " + string.Join(", ", allowedExtensions));
                }
            }
            if (products.NameImageSpecifications != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExtension = Path.GetExtension(products.NameImageSpecifications.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("NameImageSpecifications", "Chỉ chấp nhận các tệp có đuôi: " + string.Join(", ", allowedExtensions));
                }
            }
            if (ModelState.IsValid)
            {
                var load = db.Products.Where(x => x.Id == id).SingleOrDefault();
                if (load != null)
                {
                    //ImageProduct
                    if (products.NameImage != null)  // Nếu chọn hình mới
                    {
                        if (load.ImageProduct != null)
                        {
                            string filepath = Path.Combine(environment.WebRootPath, "imgs/imgProducts", load.ImageProduct);
                            if (_fileSystem.File.Exists(filepath))
                            {
                                _fileSystem.File.Delete(filepath);  //xóa ảnh cũ nếu có ảnh mới dc cập nhật
                            }
                        }
                        string tenhinh = UploadImage(products);
                        load.ImageProduct = tenhinh;

                    }
                    //ImageSpecifications
                    if (products.NameImageSpecifications != null)  // Nếu chọn hình mới
                    {
                        if (load.ImageSpecifications != null)
                        {
                            string filepath = Path.Combine(environment.WebRootPath, "imgs/imgProducts", load.ImageSpecifications);
                            if (_fileSystem.File.Exists(filepath))
                            {
                                _fileSystem.File.Delete(filepath);  //xóa ảnh cũ nếu có ảnh mới dc cập nhật
                            }
                        }
                        string tenhinh = UploadImageSpecifications(products);
                        load.ImageSpecifications = tenhinh;

                    }
                    // Cập nhật các trường khác
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
            var categories = db.Category.ToList();
            // Lấy danh sách danh mục trong category bằng seleclist để hiện ra view bằng viewbag
            ViewBag.Categories = new SelectList(categories, "Id", "Name", products.IdCategory);
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


        public async Task<IActionResult> UploadImageFromSqlToWwwRoot(int productId)
        {
            var product = await db.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            // Xác định đường dẫn lưu trữ trong wwwroot
            string uploadFolder = Path.Combine(environment.WebRootPath, "imgs/imgProducts");
            string filePath = Path.Combine(uploadFolder, product.ImageProduct);

            // Kiểm tra và tạo thư mục nếu chưa tồn tại
            Directory.CreateDirectory(uploadFolder);

            // Ghi file từ dữ liệu cơ sở dữ liệu xuống wwwroot
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Tạo MemoryStream từ dữ liệu byte của hình ảnh
                var stream = new MemoryStream(Convert.FromBase64String(product.ImageProduct));
                IFormFile file = new FormFile(stream, 0, stream.Length, "name", product.ImageProduct);

                await file.CopyToAsync(fileStream);
            }

            return RedirectToAction("Index"); // Chuyển hướng về trang chủ hoặc trang danh sách sản phẩm
        }



    }
}