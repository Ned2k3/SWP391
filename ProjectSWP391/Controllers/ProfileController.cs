using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;

namespace ProjectSWP391.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SWP391_V4Context context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProfileController(SWP391_V4Context context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        [Route("/[controller]/User-{id}")]
        public IActionResult ProfileIndex(int id)
        {
            try
            {
                var account = context.Accounts.Include(a => a.Order).Select(a => new Account
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
                var account = context.Accounts.Include(a => a.Order).Select(a => new Account
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
        public IActionResult EditProfile(IFormFile image, Account account)
        {
            try
            {
                if (image != null)
                {
                    string imagePath = SaveImage(image);
                    account.Image = imagePath;
                }

                context.Accounts.Update(account);
                context.SaveChanges();

                return RedirectToAction("ProfileIndex", new { id = account.AccountId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private string SaveImage(IFormFile image)
        {
            string uniqueFileName = null;

            if (image != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + image.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                image.CopyTo(new FileStream(filePath, FileMode.Create));
            }

            return uniqueFileName;
        }

    }
}
