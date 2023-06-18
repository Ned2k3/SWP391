﻿using ProjectSWP391.Models;
using Microsoft.AspNetCore.Mvc;

namespace DEMOSWP391.Controllers
{
    public class ProductManagementController : Controller
    {
        SWP391Context context = new SWP391Context();
        public IActionResult Index()
        {
            var lsProduct = context.Products.Include(p => p.Pcategory).ToList();
            return View(lsProduct);
        }

        public ActionResult Details(int id)
        {
            var p = context.Products.Where(n => n.ProductId == id).FirstOrDefault();
            return View(p);
        }
        //LOAD ALL CATEGORY
        public List<ProductCategory> GetProductCategories()
        {
            var list = new List<ProductCategory>();
            try
            {
                list = context.ProductCategories.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return list;
        }
        public ActionResult Create()
        {
            var categories = GetProductCategories();
            ViewBag.ListPCategory = new SelectList(categories, "PcategoryId", "PcategoryName");
            return View();
        }

        //create a product
        [HttpPost]
        public IActionResult Create(Product p)
        {
            //check product name exist
            //var existProductName = context.Products.FirstOrDefault(a => a.ProductName == p.ProductName);
            //if (existProductName != null)
            //{

            //    return RedirectToAction("Index");
            //}
            if (ModelState.IsValid)
            {
                context.Products.Add(p);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            var categories = GetProductCategories();
            ViewBag.ListPCategory = new SelectList(categories, "PcategoryId", "PcategoryName");
            return View(p);
        }
    }
}
