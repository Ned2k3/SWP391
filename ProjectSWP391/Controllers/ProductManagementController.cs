using ProjectSWP391.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;

namespace DEMOSWP391.Controllers
{
    public class ProductManagementController : Controller
    {
        private readonly SWP391_V4Context context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductManagementController(SWP391_V4Context _context, IWebHostEnvironment webHostEnvironment)
        {
            context = _context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult ProductList(string? search, bool isSearch, bool isAscendingPrice = false, int page = 1)
        {
            const int pageSize = 10;
            page = page < 1 ? 1 : page;

            var products = context.Products.Include(p => p.Pcategory).Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                Description = p.Description,
                Image = p.Image,
                IsActive = p.IsActive,
                Pcategory = p.Pcategory,
                PcategoryId = p.PcategoryId,
                Quantity = p.Quantity
            }).AsQueryable();

            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    var regex = new Regex("\\s{2,}");
                    search = regex.Replace(search.Trim(), " ");

                    products = products.Where(p => p.ProductName.Contains(search) || p.Price.ToString().Contains(search));
                }
                else if (isSearch == true && string.IsNullOrEmpty(search))
                {
                    products = products;
                }
                else
                {
                    products = products;

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var currentPageItems = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["key"] = search;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            return View(currentPageItems);
        }

        public IActionResult CreateProduct()
        {
            var pCategories = context.ProductCategories.ToList();
            ViewBag.PcategoryId = new SelectList(pCategories, "PcategoryId", "PcategoryName");
            return View();
        }

        [HttpPost]
        public IActionResult CreateProduct(ProductDTO productDTO)
        {
            productDTO.ProductName = productDTO.ProductName?.Trim();
            productDTO.Description = productDTO.Description?.Trim();
            productDTO.Image = productDTO.Image?.Trim();
            productDTO.IsActive = true;

            if (string.IsNullOrWhiteSpace(productDTO.ProductName) || productDTO.ProductName.Length < 5)
            {
                ModelState.AddModelError("ProductName", "ProductName must have at least 5 characters.");
            }
            if (productDTO.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
            }
            if (productDTO.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            }
            if (!ModelState.IsValid)
            {
                var pCategories = context.ProductCategories.ToList();
                ViewBag.PcategoryId = new SelectList(pCategories, "PcategoryId", "PcategoryName");
                return View("CreateProduct");
            }

            var product = context.Products.FirstOrDefault(prd => prd.ProductName.Trim().Equals(productDTO.ProductName));
            if (product != null)
            {
                ModelState.AddModelError("ProductName", "ProductName is already exists");
                var pCategories = context.ProductCategories.ToList();
                ViewBag.PcategoryId = new SelectList(pCategories, "PcategoryId", "PcategoryName");
                return View("CreateProduct");
            }

            var file = Request.Form.Files.FirstOrDefault();
            string? imageUrl = CreateImagePath(file);

            product = new Product()
            {
                ProductName = productDTO.ProductName,
                Quantity = productDTO.Quantity,
                Price = productDTO.Price,
                Description = productDTO.Description,
                Image = imageUrl,
                IsActive = true,
                PcategoryId = productDTO.PcategoryId
            };
            context.Products.Add(product);
            context.SaveChanges();
            return RedirectToAction(nameof(ProductList));
        }

        public string? CreateImagePath(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                // Generate a unique filename for the image
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

                // Set the physical path to save the image
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads");
                var path = Path.Combine(uploadsFolder, filename);

                // Create the "Uploads" directory if it doesn't exist
                Directory.CreateDirectory(uploadsFolder);

                // Save the file to the server
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                // Return the URL of the saved image 
                var imageUrl = Url.Content("~/Uploads/" + filename);
                return imageUrl;
            }

            // If the image upload fails, return null
            return null;
        }

        public IActionResult EditProduct(int id)
        {
            var product = context.Products.FirstOrDefault(prd => prd.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            var pCategories = context.ProductCategories.ToList();
            ViewBag.PcategoryId = new SelectList(pCategories, "PcategoryId", "PcategoryName");
            var model = new ProductDTO()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Image = product.Image,
                IsActive = true,
                Description = product.Description,
                Quantity = product.Quantity,
                Price = product.Price,
                PcategoryId = product.PcategoryId
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult EditProduct(ProductDTO productDTO)
        {
            productDTO.ProductName = productDTO.ProductName?.Trim();
            productDTO.Description = productDTO.Description?.Trim();
            productDTO.Image = productDTO.Image?.Trim();
            productDTO.IsActive = true;

            if (string.IsNullOrWhiteSpace(productDTO.ProductName) || productDTO.ProductName.Length < 5)
            {
                ModelState.AddModelError("ProductName", "ProductName must have at least 5 characters.");
                var categories = context.ProductCategories.ToList();
                ViewBag.PcategoryId = new SelectList("PcategoryId", "PcategoryName");
            }
            if (productDTO.Quantity < 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than 0");
            }
            if (productDTO.Price < 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0");
            }
            if (!ModelState.IsValid)
            {
                var pCategories = context.ProductCategories.ToList();
                ViewBag.PcategoryId = new SelectList(pCategories, "PcategoryId", "PcategoryName");
                return View("EditProduct");
            }

            var product = context.Products.FirstOrDefault(prd => prd.ProductName.Trim().Equals(productDTO.ProductName));
            if (product != null)
            {
                ModelState.AddModelError("ProductName", "ProductName is already exists");
                var pCategories = context.ProductCategories.ToList();
                ViewBag.PcategoryId = new SelectList(pCategories, "PcategoryId", "PcategoryName");
                return View("EditProduct");
            }

            product = new Product()
            {
                ProductId = productDTO.ProductId,
                ProductName = productDTO.ProductName,
                Quantity = productDTO.Quantity,
                Price = productDTO.Price,
                Description = productDTO.Description,
                Image = productDTO.Image,
                IsActive = true,
                PcategoryId = productDTO.PcategoryId
            };
            context.Entry<Product>(product).State = EntityState.Modified;
            context.SaveChanges();
            return RedirectToAction(nameof(ProductList));
        }

        public IActionResult DeactiveProduct(int id)
        {
            Product p = context.Products.FirstOrDefault(p => p.ProductId == id);
            if(p == null)
            {
                return RedirectToAction("ProductList");
            }
            if(p.IsActive == true) p.IsActive = false;
            else p.IsActive = true;
            context.SaveChanges();
            return RedirectToAction("ProductList");
        }
    }
}
