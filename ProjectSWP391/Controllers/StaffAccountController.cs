using Microsoft.AspNetCore.Mvc;
using ProjectSWP391.Models;

namespace ProjectSWP391.Controllers
{
    public class StaffAccountController : Controller
    {
        SWP391Context context = new SWP391Context();
        //action show list account of customer
        public IActionResult Index()
        {
            int role = 1;
            //show all account of Customer have role = 1;
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
                account.Role = 1;
                account.IsActive = true;
                context.Accounts.Add(account);
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }


        //xem detail cua nhan vien
        [HttpGet]
        public ActionResult Details(int id)
        {
            var s = context.Accounts.Where(n => n.AccountId == id).FirstOrDefault();
            return View(s);
        }
        [HttpGet]
        public ActionResult Update(int id)
        {
            var sID = context.Accounts.Find(id);
            return View(sID);
        }
        [HttpGet]
        public ActionResult Update(Account account)
        {
            context.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var s = context.Accounts.Where(n => n.AccountId == id).FirstOrDefault();
            return View(s);
        }

        [HttpGet]
        public ActionResult Delete(Account account)
        {
            context.Accounts.Remove(account);
            context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
