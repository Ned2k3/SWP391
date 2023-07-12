using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual Account OrderNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
