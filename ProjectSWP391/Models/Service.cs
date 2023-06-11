using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Service
    {
        public Service()
        {
            Bookings = new HashSet<Booking>();
            Feedbacks = new HashSet<Feedback>();
        }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int? ScategoryId { get; set; }

        public virtual ServiceCategory Scategory { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}
