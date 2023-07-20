using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class ServiceMaterial
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; } = null!;
        public string MaterialType { get; set; } = null!;
        public string? Image { get; set; }
        public string Suppiler { get; set; } = null!;
        public int Quantity { get; set; }
        public string Unit { get; set; } = null!;
        public decimal Price { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? ServiceId { get; set; }

        public virtual Service? Service { get; set; }
    }
}
