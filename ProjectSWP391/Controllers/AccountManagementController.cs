using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectSWP391.Models;
using System.Data;
using System.Security.Principal;

namespace ProjectSWP391.Controllers
{
    public class AccountManagementController : Controller
    {
        private readonly SWP391_V4Context context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AccountManagementController(SWP391_V4Context _context, IWebHostEnvironment webHostEnvironment)
        {
            context = _context;
            _webHostEnvironment = webHostEnvironment;
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult AccountList(string? search)
        {
            if (string.IsNullOrEmpty(search))
            {
                List<Account> accounts = context.Accounts.Where(a => a.AccountId != 0 && a.Role != 1).ToList();

                return View(accounts);
            }
            else
            {
                search.Trim();
                var acc = context.Accounts.Where(p => p.FullName.Contains(search)).ToList();
                ViewData["key"] = search;
                return View(acc);
            }


        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                ViewBag.ErrorMsg = "You cant change this user";
                return View();
            }
            var a = context.Accounts.Where(ac => ac.AccountId == id).SingleOrDefault();

            if (a.IsActive == 0)
            {
                a.IsActive = 1;
            }
            else if (a.IsActive == 1)
            {
                a.IsActive = 0;
            }
            //context.Remove(a);
            context.Update(a);
            context.SaveChanges();
            return RedirectToAction("AccountList");
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

        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpGet]
        public IActionResult Update(int id)
        {

            if (id == 0)
            {
                ViewBag.ErrorMsg = "You cant change this user";
                return View();
            }
            var a = context.Accounts.SingleOrDefault(v => v.AccountId == id);
            return View(a);
        }


        [HttpPost]
        public IActionResult Update(Account account)
        {
            account.Email = account.Email.Trim();
            account.FullName = account.FullName.Trim();
            account.Password = EncryptionHelper.Encrypt(account.Password);
            account.Role = 2;
            account.IsActive = 1;
            var file = Request.Form.Files.FirstOrDefault();
            string? imageUrl = CreateImagePath(file);
            account.Image = imageUrl;
            context.Update(account);
            context.SaveChanges();
            ViewBag.SuccessMsg = "SaveChange Successed";
            return RedirectToAction("AccountList");
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        public IActionResult Detail(string email)
        {
            var a = context.Accounts.SingleOrDefault(i => i.Email == email);
            return View(a);
        }

        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(AuthenticationSchemes = "Auth", Roles = "1")]
        [HttpPost]
        public IActionResult Create(Account account)
        {
            if (account == null)
            {
                ViewBag.ErrorMsg = "Account input field";
                return View();
            }
            else
            {
                var a = context.Accounts.Where(c => c.Email == account.Email).SingleOrDefault();

                if (a == null)
                {

                    account.Email = account.Email.Trim();
                    account.FullName = account.FullName.Trim();
                    account.Password = EncryptionHelper.Encrypt(account.Password);
                    //let choose role
                    if (account.Role == 3)
                    {
                        account.Role = null;
                    }

                    context.Add(account);
                    context.SaveChanges();

                }
                else
                {
                    ViewBag.ErrorMsg = "Account have existed please enter a different Account/Email!";
                    return View();
                }
                return RedirectToAction("AccountList");
            }
        }

    }
}
