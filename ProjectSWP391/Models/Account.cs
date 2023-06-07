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
        }
        //phone va role la null real
        public int? AccountId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format. Please enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
        public int? Phone { get; set; }
        public string? FullName { get; set; }
        public int? Role { get; set; }
        public bool? IsActive { get; set; }
        public string? Image { get; set; }

        public virtual Order Order { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Booking> BookingCustomers { get; set; }
        public virtual ICollection<Booking> BookingEmployees { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
