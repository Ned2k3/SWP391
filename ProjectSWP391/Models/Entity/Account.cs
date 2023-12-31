﻿using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models.Entity
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

        public int AccountId { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int? Phone { get; set; }
        public string? FullName { get; set; }
        public int? Role { get; set; }
        public bool IsActive { get; set; }
        public string? Image { get; set; }

        public virtual Order? Order { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Booking> BookingCustomers { get; set; }
        public virtual ICollection<Booking> BookingEmployees { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
