using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Completion;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

        //Used to load the list of selected service
        public List<ServiceList>? GetSelectedService()
        {
            var session = HttpContext.Session;
            //create a new list of service selection
            List<ServiceList>? selectionList = new List<ServiceList>();
            string? serializedList = session.GetString("selectionList");
            if (serializedList != null)
            {
                selectionList = JsonConvert.DeserializeObject<List<ServiceList>>(serializedList);
            }
            return selectionList;
        }

        public void SaveSelectedService(List<ServiceList> selectionList)
        {
            var session = HttpContext.Session;
            //set the booking serviceList to selectionList
            booking.ServiceLists = selectionList;
            //Save the selection list
            string serviceList = JsonConvert.SerializeObject(selectionList);
            session.SetString("selectionList", serviceList);
        }
        //This function is use to add selected service in every step when user book service
        public void AddSelectedService(int sID)
        {
            using (var context = new SWP391_V4Context())
            {
                var session = HttpContext.Session;
                if (sID != 0)
                {
                    //if the adding service is not in the list
                    List<ServiceList> selectionList = GetSelectedService();
                    if (!selectionList.Where(s => s.Service.ServiceId == sID).Any())
                    {
                        Service? sv = context.Services.Where(i => i.ServiceId == sID).FirstOrDefault();
                        if (sv != null)
                        {
                            //Add the new selected service
                            ServiceList sl = new ServiceList();
                            sl.Service = sv;
                            selectionList.Add(sl);
                            SaveSelectedService(selectionList);
                        }
                    }
                    else
                    {
                        SaveSelectedService(selectionList);
                    }
                }
            }
        }

        //This function is use to remove selected service in every step when user book service
        public void RemoveSelectedService(int sID)
        {
            using (var context = new SWP391_V4Context())
            {
                var session = HttpContext.Session;
                if (sID != 0)
                {
                    //if the adding service is not in the list
                    List<ServiceList> selectionList = GetSelectedService();
                    ServiceList? sl = selectionList.Where(s => s.Service.ServiceId == sID).FirstOrDefault();
                    if (sl != null)
                    {
                        selectionList.Remove(sl);
                        SaveSelectedService(selectionList);
                    }
                    else
                    {
                        SaveSelectedService(selectionList);
                    }
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
                using var context = new SWP391_V4Context();
                var acc = context.Accounts.Where(a => a.Phone == phone && a.Role == null).FirstOrDefault();
                if (acc != null)
                {
                    booking.Customer = acc;
                }
                else
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
            using (var context = new SWP391_V4Context())
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
            using (var context = new SWP391_V4Context())
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
            using (var context = new SWP391_V4Context())
            {
                //if there user click button from a service card
                if (sID != 0)
                {
                    //if guest go to another page, they will have to fill information again
                    if (step == 2)
                    {
                        AddSelectedService(sID);
                    }
                    else if (step == 3)
                    {
                        RemoveSelectedService(sID);
                    }
                    else
                    {
                        session.Remove("phone");
                        session.Remove("bookingDate");
                        List<ServiceList>? selectionList = GetSelectedService();
                        selectionList.Clear();
                        SaveSelectedService(selectionList);
                        AddSelectedService(sID);
                    }
                }
                else
                {
                    //load the current selected service list
                    List<ServiceList>? selectionList = GetSelectedService();
                    SaveSelectedService(selectionList);
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
        public Booking? GetCurrentBooking(int phone)
        {
            using (var context = new SWP391_V4Context())
            {
                var account = context.Accounts.Where(a => a.Phone == phone && a.Role == null).FirstOrDefault();
                Booking? bk;
                if (account != null)
                {
                    bk = context.Bookings.Where(b => ((b.BookingDate.Date == DateTime.Today.Date && b.Shift > DateTime.Now.Hour)
                         || (b.BookingDate.Date > DateTime.Today.Date)) && b.CustomerId == account.AccountId).FirstOrDefault();
                }
                else
                {
                    bk = context.Bookings.Where(b => ((b.BookingDate.Date == DateTime.Today.Date && b.Shift > DateTime.Now.Hour)
                         || (b.BookingDate.Date > DateTime.Today.Date)) && b.CustomerId == 0 && b.Content.Contains(phone.ToString())).FirstOrDefault();
                }
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
                    Booking? bk = GetCurrentBooking(phoneNumber);
                    if (bk != null) return Redirect($"/CustomerManagement/LandingPage?booked={bk.BookingId}");
                    if (session.GetString("phone") != null)
                    {
                        session.Clear();
                    }
                    using (var context = new SWP391_V4Context())
                    {
                        var account = context.Accounts.Where(a => a.Phone == phoneNumber && a.Role == null).FirstOrDefault();
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
                Booking? bk = GetCurrentBooking(phoneNumber);
                if (bk != null)
                {
                    ViewBag.currentBooking = bk;
                }
                else
                {
                    if (session.GetString("phone") != null)
                    {
                        session.Clear();
                    }
                    string? name = Request.Form["fullname"].ToString().Trim();
                    string? email = Request.Form["email"].ToString().Trim();
                    session.SetString("phone", phoneNumber.ToString());
                    using (var context = new SWP391_V4Context())
                    {
                        var account = context.Accounts.Where(a => a.Phone == phoneNumber && a.Role == null).FirstOrDefault();
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
                                acc.FullName = Global.Capitalize(name);
                            }
                            if (email != null)
                            {
                                session.SetString("email", email);
                                acc.Email = email;
                            }
                            booking.Customer = acc;
                        }
                        List<ServiceList> sl = GetSelectedService();
                        SaveSelectedService(sl);
                    }
                }
            }
            return View("/Views/Booking/Load.cshtml", booking);
        }

        //Load the service list to select service from booking main screen
        public IActionResult LoadServiceSelection(int serviceID)
        {
            using (var context = new SWP391_V4Context())
            {
                List<ServiceList>? selectionList = GetSelectedService();
                ViewBag.selectedServiceID = selectionList;
                List<Service> services = context.Services.OrderBy(s => s.ServiceId).ToList();
                List<ServiceCategory> categories = context.ServiceCategories.OrderBy(s => s.ScategoryId).ToList();
                ViewBag.serviceCategories = categories;
                return View(services);
            }
        }

        //This function is use to save booking when user confirm
        public IActionResult ConfirmBooking(Booking bk)
        {
            using (var context = new SWP391_V4Context())
            {
                //Add new record for booking
                context.Bookings.Add(bk);
                context.SaveChanges();
                //Get current booking ID
                Booking? currentBooking = context.Bookings.OrderByDescending(b => b.BookingId).FirstOrDefault();
                int id = 1;
                if (currentBooking != null)
                {
                    id = currentBooking.BookingId;
                }
                //Add new record for service selection
                List<ServiceList>? selectionList = GetSelectedService();
                if (selectionList != null)
                {
                    foreach (var item in selectionList)
                    {
                        ServiceList sl = new ServiceList()
                        {
                            BookingId = id,
                            ServiceId = item.Service.ServiceId,
                        };
                        context.ServiceLists.Add(sl);
                    }
                }
                context.SaveChanges();
                string[] msg = { "Booking Complete", "Enter phone number again to view details" };
                return RedirectToAction("LandingPage", "CustomerManagement", new {message = msg});
            }
        }

        //View the booking details
        public IActionResult BookingDetails(int bookingID)
        {
            using (var context = new SWP391_V4Context())
            {
                Booking? bk = context.Bookings.Where(b => b.BookingId == bookingID).FirstOrDefault();
                if (bk != null)
                {
                    //Get the servic list of a booking
                    var selectionList = context.ServiceLists.Where(s => s.BookingId == bk.BookingId).Select(s => new
                    {
                        ServiceName = s.Service.ServiceName,
                        Price = s.Service.Price
                    }).ToList();
                    if (selectionList != null)
                    {
                        ViewBag.selectionList = selectionList;
                    }
                    //Get the account infomation of a booking
                    Account? emp = context.Accounts.Where(a => a.AccountId == bk.EmployeeId).FirstOrDefault();
                    bk.Employee = emp;
                    if (bk.CustomerId != 0)
                    {
                        Account? cus = context.Accounts.Where(a => a.AccountId == bk.CustomerId).FirstOrDefault();
                        bk.Customer = cus;
                    }
                }
                return View(bk);
            }
        }
    }
}
