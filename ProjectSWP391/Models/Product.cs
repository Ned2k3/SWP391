using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProjectSWP391.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int ProductId { get; set; }
       
        [StringLength(100)]
        public string ProductName { get; set; } = null!;
      
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
        public int PcategoryId { get; set; }
        public bool IsActive { get; set; }

        public virtual ProductCategory Pcategory { get; set; } = null!;
        public virtual Feedback? Feedback { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
