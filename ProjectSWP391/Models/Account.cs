using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectSWP391.Models
{
    public partial class Account
    {
        public Account()
        {
            Blogs = new HashSet<Blog>();
            BookingCustomers = new HashSet<Booking>();
            BookingEmployees = new HashSet<Booking>();
            Feedbacks = new HashSet<Feedback>();
            IsWorkings = new HashSet<IsWorking>();
            Orders = new HashSet<Order>();
        }

        public int AccountId { get; set; }
       
        [StringLength(50)]
        public string Email { get; set; } = null!;
        
        [StringLength(20)]
        public string Password { get; set; } = null!;
        
        public int? Phone { get; set; }
       
        [StringLength(50)]
        public string? FullName { get; set; }
        public int? Role { get; set; }
        public int IsActive { get; set; }
        public string? Image { get; set; }

        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Booking> BookingCustomers { get; set; }
        public virtual ICollection<Booking> BookingEmployees { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<IsWorking> IsWorkings { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
