using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;

namespace ProjectSWP391.Controllers
{
    public class ProfileController : Controller
    {
        private readonly SWP391_V4Context context;

        public ProfileController(SWP391_V4Context context)
        {
            this.context = context;
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
        public IActionResult EditProfile(IFormFile image)
        {

            return View(image);
        }

        public IActionResult CompleteCustomerProfile(int id)
        {
            Account acc = context.Accounts.FirstOrDefault(a => a.AccountId == id);
            return View(acc);
        }
    }
}
