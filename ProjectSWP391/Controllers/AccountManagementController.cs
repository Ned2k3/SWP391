using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ProjectSWP391.Models;
using System.Security.Principal;

namespace ProjectSWP391.Controllers
{
    public class AccountManagementController : Controller
    {
        private readonly SWP391_V4Context context;


        public AccountManagementController(SWP391_V4Context _context)
        {
            context = _context;
        }

        public IActionResult AccountList()
        {
            List<Account> accounts = context.Accounts.Where(a => a.AccountId != 0 && a.Role != 1).ToList();
            return View(accounts);
            
        }
        public IActionResult Delete(int id)
        {
            if (id == 0)
            {
                ViewBag.ErrorMsg = "You cant change this user";
                return View();
            }
            var a = context.Accounts.Where(ac => ac.AccountId == id).SingleOrDefault();
            context.Remove(a);
            context.SaveChanges();
            return RedirectToAction("AccountList");
        }
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
            if (ModelState.IsValid)
            {
               
                
                if (account.Role == 3)
                {
                    account.Role = null;
                }
                account.Password = EncryptionHelper.Encrypt(account.Password);
                context.Update(account);
                context.SaveChanges();
                ViewBag.SuccessMsg = "SaveChange Successed";

            }
            return View(account);

        }
        public IActionResult Detail(string email)
        {
            var a = context.Accounts.SingleOrDefault(i => i.Email == email);
            return View(a);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Account account)
        {
            if(account == null)
            {
                ViewBag.ErrorMsg = "Account input field";
                return View();
            }
            else
            {
                var a = context.Accounts.Where(c => c.Email == account.Email).SingleOrDefault();
                
                if (a == null)
                {
                    account.Password = EncryptionHelper.Encrypt(account.Password);
                    //let choose role
                    if(account.Role == 3) { account.Role=null; }
                    
                    context.Add(account);
                    context.SaveChanges();
                }
                return RedirectToAction("AccountList");
            }
        }
        
    }
}
