using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Shift
    {
        public Shift()
        {
            Bookings = new HashSet<Booking>();
        }

        public int ShiftId { get; set; }
        public DateTime ShiftDay { get; set; }
        public int ShiftNumber { get; set; }
        //shift 1: 8h - 10h
        //shift 2: 10h - 12h
        //shift 3: 1h - 13h
        //shift 4: 13h - 15h
        //shift 5: 15h - 17h
        //shift 6: 17h - 19h

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
