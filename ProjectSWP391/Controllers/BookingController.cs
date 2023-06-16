using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using ProjectSWP391.Models;
using ProjectSWP391.Models.ServiceModel;
using System.Runtime.Versioning;
using System.Security.Cryptography;

namespace ProjectSWP391.Controllers
{

    public class BookingController : Controller
    {
        [BindProperty]
        private Booking booking { get; set; } = new Booking();

        //This function is use to update selected service in every step when user book service
        public void SaveSelectedService(int sID)
        {
            using (var context = new SWP391Context())
            {
                var session = HttpContext.Session;
                Service? sv = context.Services.Where(i => i.ServiceId == sID).FirstOrDefault();
                if (sv != null)
                {
                    booking.Service = sv;
                    session.SetString("serviceID", sID.ToString());
                }
            }
        }

        //This function is use to get the current user booking service
        public void GetUserBySession()
        {
            var session = HttpContext.Session;
            int phone = session.GetString("phone") == null ? 0 : Convert.ToInt32(session.GetString("phone"));
            if (phone != 0)
            {
                Account account = new Account()
                {
                    Phone = phone
                };

                string? fname = session.GetString("fname") == null ? null : session.GetString("fname");
                if (fname != null)
                {
                    account.FullName = fname;
                }
                string? email = session.GetString("email") == null ? null : session.GetString("email");
                if (email != null)
                {
                    account.Email = email;
                }
                booking.Customer = account;
            }
        }

        //This function is use to save and get the booking date of user
        public void SaveBookingDate(string? bookingDate)
        {
            var session = HttpContext.Session;
            if (bookingDate != null)
            {
                booking.BookingDate = DateTime.Parse(bookingDate);
                session.SetString("bookingDate", bookingDate);
                session.Remove("bookingShift");
            }
            else
            {
                if (session.GetString("bookingDate") != null)
                {
                    string? bkDate = session.GetString("bookingDate");
                    booking.BookingDate = DateTime.Parse(bkDate);
                }
                else
                {
                    booking.BookingDate = DateTime.Today;
                    session.SetString("bookingDate", DateTime.Today.ToString());
                    session.Remove("bookingShift");
                }
            }
        }

        //This function is use to save and get the booking shift of user
        public void SaveBookingShift(int shift)
        {
            var session = HttpContext.Session;
            if (shift != 0)
            {
                booking.Shift = shift;
                session.SetString("bookingShift", shift.ToString());
                session.Remove("employeeID");
            }
            else
            {
                var bkShift = session.GetString("bookingShift");
                if (bkShift != null)
                {
                    booking.Shift = Convert.ToInt32(bkShift);
                }
                else
                {
                    session.Remove("employeeID");
                }
            }
        }

        //this function is use to load the available employee order by shift and date
        public List<Account>? LoadAvailableEmployee()
        {
            var session = HttpContext.Session;
            string? bookingDate = session.GetString("bookingDate");
            var shift = session.GetString("bookingShift");
            if (shift == null)
            {
                return null;
            }
            using (var context = new SWP391Context())
            {
                List<Booking> bookingList = new List<Booking>();
                var bookings = context.Bookings.ToList();
                foreach (Booking item in bookings)
                {
                    if (item.BookingDate.ToString().Equals(bookingDate) && item.Shift == Convert.ToInt32(shift))
                    {
                        bookingList.Add(item);
                    }
                }
                var accounts = context.Accounts.ToList();
                var queries = (from account in accounts
                               join booking in bookingList on account.AccountId equals booking.EmployeeId
                               select account).Distinct();
                var result = from account in accounts
                             where account.Role == 2
                             join query in queries on account.AccountId equals query.AccountId into joinedEntities
                             from target in joinedEntities.DefaultIfEmpty()
                             where target == null
                             select account;
                return result.ToList();
            }
        }

        //this function is use to save and get the employeeID selected
        public void SaveBookingEmployee(int empID)
        {
            using (var context = new SWP391Context())
            {
                var session = HttpContext.Session;
                if (empID == 0)
                {
                    if (session.GetString("employeeID") != null)
                    {
                        empID = Convert.ToInt32(session.GetString("employeeID"));
                        Account? emp = context.Accounts.Where(a => a.AccountId == empID).FirstOrDefault();
                        booking.Employee = emp;
                        session.SetString("employeeID", empID.ToString());
                    }
                }
                else
                {
                    Account? emp = context.Accounts.Where(a => a.AccountId == empID).FirstOrDefault();
                    if (emp != null)
                    {
                        booking.Employee = emp;
                        session.SetString("employeeID", empID.ToString());
                    }
                }
            }
        }


        //Load the main screen for booking service 
        public IActionResult Load(int sID, string? bookingDate, int shift, int step, int empID)
        {
            var session = HttpContext.Session;

            Account? acc = Global.CurrentUser;
            List<DateTime> dates = Global.GetSchedule();
            ViewBag.schedule = dates;

            //Handle service selection here
            using (var context = new SWP391Context())
            {
                if (sID != 0)
                {
                    if (step != 2)
                    {
                        session.Remove("phone");
                        session.Remove("bookingDate");
                    }
                    SaveSelectedService(sID);
                }
                else
                {
                    int serviceID = session.GetString("serviceID") == null ? 0 : Convert.ToInt32(session.GetString("serviceID"));
                    SaveSelectedService(serviceID);
                }
            }

            //Handle the customer account here
            if (acc != null)
            {
                booking.Customer = acc;
            }
            else GetUserBySession();

            //Handle date selection here
            SaveBookingDate(bookingDate);

            //handle shift selection here
            SaveBookingShift(shift);

            //Load the employee list order by shift and date
            ViewBag.empList = LoadAvailableEmployee();

            //Handle staff selection here
            SaveBookingEmployee(empID);
            return View(booking);
        }

        //check if user has booked before or not
        public Booking? GetCurrentBooking(int? cusID, int phone)
        {
            using (var context = new SWP391Context())
            {
                Booking? bk = context.Bookings.Where(b => ((b.BookingDate.Date == DateTime.Today.Date && b.Shift > DateTime.Now.Hour)
                || (b.BookingDate.Date > DateTime.Today.Date)) && b.Content.Contains(phone.ToString())).FirstOrDefault();

                if (bk != null)
                {
                    return bk;
                }
                return null;
            }
        }
        //Load the main screen booking service when user enter information
        [HttpPost]
        public IActionResult LoadByPost()
        {
            var session = HttpContext.Session;

            //check if step is 0 or 1
            int step = (Request.Form["step"].ToString() != null ? Convert.ToInt32(Request.Form["step"]) : 0);
            List<DateTime> dates = Global.GetSchedule();
            ViewBag.schedule = dates;
            booking.BookingDate = DateTime.Today;

            //guest enter phone number from landing page
            if (step == 0)
            {
                if (Global.CurrentUser != null)
                {
                    booking.Customer = Global.CurrentUser;
                }
                else
                {
                    int phoneNumber = Convert.ToInt32(Request.Form["phone"]);
                    Booking? bk = GetCurrentBooking(0, phoneNumber);
                    if (bk != null) return Redirect($"/CustomerManagement/LandingPage?booked={bk.BookingId}");
                    if (session.GetString("phone") != null)
                    {
                        session.Clear();
                    }
                    using (var context = new SWP391Context())
                    {
                        var account = context.Accounts.Where(a => a.AccountId == phoneNumber).FirstOrDefault();
                        session.SetString("phone", phoneNumber.ToString());
                        if (account != null)
                        {
                            booking.Customer = account;
                        }
                        else
                        {
                            Account? acc = new Account()
                            {
                                Phone = phoneNumber
                            };
                            booking.Customer = acc;
                        }
                    }
                }
            }

            //guest book from service list and have to fill information
            if (step == 1)
            {
                int phoneNumber = Convert.ToInt32(Request.Form["phone"]);
                Booking? bk = GetCurrentBooking(0, phoneNumber);
                if (bk != null)
                {
                    ViewBag.currentBooking = bk;
                }
                else
                {
                    int sID = Convert.ToInt32(session.GetString("serviceID"));
                    string? name = Request.Form["fname"].ToString().Trim();
                    string? email = Request.Form["email"].ToString().Trim();
                    session.SetString("phone", phoneNumber.ToString());
                    using (var context = new SWP391Context())
                    {
                        var account = context.Accounts.Where(a => a.Phone == phoneNumber).FirstOrDefault();
                        Service? sv = context.Services.Where(i => i.ServiceId == sID).FirstOrDefault();
                        if (account != null)
                        {
                            booking.Customer = account;
                        }
                        else
                        {
                            Account? acc = new Account()
                            {
                                Phone = phoneNumber
                            };
                            if (name != null)
                            {
                                session.SetString("fname", name);
                                acc.FullName = name;
                            }
                            if (email != null)
                            {
                                session.SetString("email", email);
                                acc.Email = email;
                            }
                            booking.Customer = acc;
                        }
                        if (sv != null)
                        {
                            booking.Service = sv;
                        }
                    }
                }
            }
            return View("/Views/Booking/Load.cshtml", booking);
        }

        //Load the service list to select service from booking main screen
        public IActionResult LoadServiceSelection(int serviceID)
        {
            using (var context = new SWP391Context())
            {
                if (serviceID != 0)
                {
                    ViewBag.selectedServiceID = serviceID;
                }
                List<Service> services = context.Services.OrderBy(s => s.ServiceId).ToList();
                List<ServiceCategory> categories = context.ServiceCategories.OrderBy(s => s.ScategoryId).ToList();
                ViewBag.serviceCategories = categories;
                return View(services);
            }
        }

        //This function is use to save booking when user confirm
        public IActionResult ConfirmBooking(Booking bk)
        {
            using (var context = new SWP391Context())
            {
                Booking? currentBk = context.Bookings.OrderByDescending(b => b.BookingId).FirstOrDefault();
                if (currentBk != null)
                {
                    int nextID = currentBk.BookingId + 1;
                    bk.BookingId = nextID;
                }
                else
                {
                    bk.BookingId = 1;
                }
                context.Bookings.Add(bk);
                context.SaveChanges();
                return RedirectToAction("LandingPage", "CustomerManagement");
            }
        }

        //View the booking details
    }
}
