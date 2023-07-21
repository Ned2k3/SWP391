using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using ServiceMaterialController.Models;
using ServiceMaterialController.Models.ServiceMaterialDTO;
using System.Data;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace ServiceMaterialController.Controllers
{
    public class ServiceMaterialManagement : Controller
    {
        private readonly SWP391_V4Context context;

        public ServiceMaterialManagement(SWP391_V4Context _context)
        {
            context = _context;
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Index(string? search, bool isSearch, bool isAscendingPrice = false, int page = 1)
        {
            const int pageSize = 10;
            page = page < 1 ? 1 : page;

            var materials = context.ServiceMaterials.Include(sm => sm.Service).Select(sm => new ServiceMaterialDTO
            {
                MaterialId = sm.MaterialId,
                MaterialName = sm.MaterialName,
                MaterialType = sm.MaterialType,
                Image = sm.Image,
                Suppiler = sm.Suppiler,
                Quantity = sm.Quantity,
                Unit = sm.Unit,
                Price = sm.Price,
                ExpiryDate = sm.ExpiryDate,
                CreatedDate = sm.CreatedDate,
                UpdatedDate = sm.UpdatedDate,
                ExpiringSoon = sm.ExpiryDate.HasValue && sm.ExpiryDate.Value >= DateTime.Now.Date.AddDays(-1),
                ServiceId = sm.ServiceId,
                Service = sm.Service
            }).AsQueryable();
            try
            {
                if (!string.IsNullOrEmpty(search))
                {
                    var regex = new Regex("\\s{2,}");
                    search = regex.Replace(search.Trim(), " ");

                    materials = materials.Where(s => s.MaterialName.Contains(search.Trim()) || s.Price.ToString().Contains(search));
                }
                else if (isSearch == true && string.IsNullOrEmpty(search))
                {
                    materials = materials.Where(s => false);
                }
                else
                {
                    materials = materials.OrderByDescending(s => s.ExpiryDate.HasValue && s.ExpiryDate.Value < DateTime.Now.Date).ThenBy(s => s.CreatedDate);

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            var totalItems = materials.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            var currentPageItems = materials
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewData["key"] = search;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            ViewBag.IsAscendingPrice = isAscendingPrice; // OrderBy Price for ServiceView
            return View(currentPageItems);
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Create()
        {
            var services = context.Services.ToList();
            ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
            return View();
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ServiceMaterialDTO serviceMaterial)
        {
            serviceMaterial.MaterialName = serviceMaterial.MaterialName?.Trim();
            serviceMaterial.MaterialType = serviceMaterial.MaterialType?.Trim();
            serviceMaterial.Suppiler = serviceMaterial.Suppiler?.Trim();

            if (string.IsNullOrWhiteSpace(serviceMaterial.MaterialName) || serviceMaterial.MaterialName.Length < 5)
            {
                ModelState.AddModelError("MaterialName", "Material Name must have at least 5 characters.");
            }

            if (serviceMaterial.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
            }
            if (serviceMaterial.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            }
            if (serviceMaterial.ExpiryDate.HasValue)
            {
                if (serviceMaterial.ExpiryDate.Value <= DateTime.Now.Date)
                {
                    ModelState.AddModelError("ExpiryDate", "Expiry Date must be in the future.");
                }
            }
            if (!ModelState.IsValid)
            {
                var services = context.Services.ToList();
                ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
                return View("Create");
            }

            var smat = context.ServiceMaterials.FirstOrDefault(sm => sm.MaterialName == serviceMaterial.MaterialName); ;
            if (smat != null)
            {
                ModelState.AddModelError("MaterialName", "Material Name is already exists.");
                var services = context.Services.ToList();
                ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
                return View("Create");
            }

            ServiceMaterial sm = context.ServiceMaterials.FirstOrDefault(sm => sm.MaterialName.Trim().Equals(serviceMaterial.MaterialName));

            if (sm != null)
            {
                var services = context.Services.ToList();
                ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
                return View("Create");
            }

            sm = new ServiceMaterial()
            {
                MaterialId = serviceMaterial.MaterialId,
                MaterialName = serviceMaterial.MaterialName,
                MaterialType = serviceMaterial.MaterialType,
                Price = serviceMaterial.Price,
                Image = serviceMaterial.Image,
                Suppiler = serviceMaterial.Suppiler,
                Quantity = serviceMaterial.Quantity,
                Unit = serviceMaterial.Unit,
                ExpiryDate = serviceMaterial.ExpiryDate,
                CreatedDate = DateTime.Now,
                UpdatedDate = DateTime.Now,
                ServiceId = serviceMaterial.ServiceId
            };
            context.ServiceMaterials.Add(sm);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Edit(int id)
        {
            var serviceMaterial = context.ServiceMaterials.FirstOrDefault(sm => sm.MaterialId == id);
            if (serviceMaterial == null)
            {
                return NotFound();
            }
            var services = context.Services.ToList();
            ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
            var model = new ServiceMaterialDTO
            {
                MaterialId = serviceMaterial.MaterialId,
                MaterialName = serviceMaterial.MaterialName,
                MaterialType = serviceMaterial.MaterialType,
                Image = serviceMaterial.Image,
                Suppiler = serviceMaterial.Suppiler,
                Quantity = serviceMaterial.Quantity,
                Unit = serviceMaterial.Unit,
                Price = serviceMaterial.Price,
                ExpiryDate = serviceMaterial.ExpiryDate,
                ServiceId = serviceMaterial.ServiceId
            };
            if (model.IsExpired)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Edit(ServiceMaterialDTO serviceMaterial)
        {
            serviceMaterial.MaterialName = serviceMaterial.MaterialName?.Trim();
            serviceMaterial.MaterialType = serviceMaterial.MaterialType?.Trim();
            serviceMaterial.Suppiler = serviceMaterial.Suppiler?.Trim();

            if (string.IsNullOrWhiteSpace(serviceMaterial.MaterialName) || serviceMaterial.MaterialName.Length < 5)
            {
                ModelState.AddModelError("MaterialName", "Material Name must have at least 5 characters.");
            }

            if (serviceMaterial.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Quantity must be greater than 0.");
            }
            if (serviceMaterial.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                var services = context.Services.ToList();
                ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
                return View("Edit");
            }
            if (serviceMaterial.ExpiryDate.HasValue)
            {
                if (serviceMaterial.ExpiryDate.Value <= DateTime.Now.Date)
                {
                    ModelState.AddModelError("ExpiryDate", "Expiry Date must be in the future.");
                }
            }
            var smat = context.ServiceMaterials.FirstOrDefault(sm => sm.MaterialName.Trim().Equals(serviceMaterial.MaterialName));
            if (smat != null)
            {
                ModelState.AddModelError("MaterialName", "Material Name is already exists.");
                var services = context.Services.ToList();
                ViewBag.ServiceId = new SelectList(services, "ServiceId", "ServiceName");
                return View("Edit");
            }

            ServiceMaterial sm = new ServiceMaterial()
            {
                MaterialId = serviceMaterial.MaterialId,
                MaterialName = serviceMaterial.MaterialName,
                MaterialType = serviceMaterial.MaterialType,
                Price = serviceMaterial.Price,
                Image = serviceMaterial.Image,
                Suppiler = serviceMaterial.Suppiler,
                Quantity = serviceMaterial.Quantity,
                Unit = serviceMaterial.Unit,
                ExpiryDate = serviceMaterial.ExpiryDate,
                UpdatedDate = DateTime.Now,
                ServiceId = serviceMaterial.ServiceId
            };
            context.Entry<ServiceMaterial>(sm).State = EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Delete(int id)
        {
            var sm = context.ServiceMaterials.FirstOrDefault(context => context.MaterialId == id);
            if (sm == null) return NotFound();
            context.Remove(sm);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
