﻿using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models.Entity
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public int Amount { get; set; }
        public int ProductId { get; set; }
        public DateTime? OrderDate { get; set; }

        public virtual Account OrderNavigation { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
