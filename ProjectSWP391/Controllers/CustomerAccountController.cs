using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;

namespace ProjectSWP391.Controllers
{
    public class CustomerAccountController : Controller
    {
        SWP391Context context = new SWP391Context();
        //action show list account of customer
        public IActionResult Index()
        {
            int role = 2;
            //show all account of Customer have role = 2;
            var lstCustomer = context.Accounts.Where(a => a.Role == role).ToList();
            return View(lstCustomer);
        }

        //action show form create account
        public IActionResult Create()
        {
            return View();
        }

        //action logic processing create new account 
        [HttpPost]
        public ActionResult Create(Account account)
        {
            //check email exist
            var existEmail = context.Accounts.FirstOrDefault(a => a.Email == account.Email);
            if (existEmail != null)
            {
                ModelState.AddModelError("Email", "Email exist, please enter new email");
                return View(account);
            }
            if (ModelState.IsValid)
            {
                account.Role = 2;
                account.IsActive = true;
                context.Accounts.Add(account);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        //xem detail cua khach
        [HttpGet]
        public ActionResult Details(int id)
        {
            var s = context.Accounts.Where(n => n.AccountId == id).FirstOrDefault();
            return View(s);
        }

        public IActionResult Update(int id)
        {
            try
            {
                Account account = context.Accounts.SingleOrDefault(a => a.AccountId == id);
                if (account == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(account);
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);

            }

        }
        [HttpPost]
        public IActionResult Update(Account account)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Update", account.AccountId);
                }
                context.Accounts.Update(account);
                context.SaveChanges();

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                Account account = context.Accounts.SingleOrDefault(a => a.AccountId == id);
                if (account == null)
                {
                    return NotFound();
                }
                context.Accounts.Remove(account);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("Index");
        }
    }
}
