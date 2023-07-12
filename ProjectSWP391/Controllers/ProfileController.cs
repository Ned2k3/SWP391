﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;

namespace ProjectSWP391.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SWP391_V4Context context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(SWP391_V4Context context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [Route("/[controller]/User-{id}")]
        public IActionResult ProfileIndex(int id)
        {
            try
            {
                var account = context.Accounts.Include(a => a.Orders).Select(a => new Account
                {
                    AccountId = a.AccountId,
                    Email = a.Email,
                    Password = a.Password,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    Role = a.Role,
                    Image = a.Image,
                }).SingleOrDefault(a => a.AccountId == id);
                if (account == null)
                {
                    return NotFound();
                }
                ViewBag.UserInfor = account;
                return View();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/[controller]/User-{id}/EditProfile")]
        public IActionResult EditProfile(int id) {

            try
            {
                var account = context.Accounts.Include(a => a.Orders).Select(a => new Account
                {
                    AccountId = a.AccountId,
                    Email = a.Email,
                    Password = a.Password,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    Role = a.Role,
                    Image = a.Image,
                }).SingleOrDefault(a => a.AccountId == id);
                if (account == null)
                {
                    return NotFound();
                }
                ViewBag.UserInfor = account;
                return View();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult EditProfile(IFormFile image)
        {

            return View(image);
        }

        public IActionResult CompleteCustomerProfile(int id)
        {
            Account acc = context.Accounts.FirstOrDefault(a => a.AccountId == id);
            return View(acc);
        }

        [HttpPost]
        public IActionResult CompleteCustomerProfile(Account account)
        {
            Account acc = context.Accounts.FirstOrDefault(a => a.AccountId == account.AccountId);

            string fname = Request.Form["FullName"];
            var file = Request.Form.Files.FirstOrDefault();
            string? imageUrl = CreateImagePath(file);

            acc.FullName = fname;
            acc.Phone = account.Phone;
            acc.Image = imageUrl;
            context.SaveChanges();
            Global.CurrentUser = acc;
            return RedirectToAction("LandingPage","CustomerManagement");
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
    }
}
