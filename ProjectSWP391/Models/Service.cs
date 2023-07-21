using System;
using System.Collections.Generic;

namespace ProjectSWP391.Models
{
    public partial class Service
    {
        public Service()
        {
            Feedbacks = new HashSet<Feedback>();
            ServiceLists = new HashSet<ServiceList>();
            ServiceMaterials = new HashSet<ServiceMaterial>();
        }

        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int ScategoryId { get; set; }

        public virtual ServiceCategory? Scategory { get; set; }
        public virtual ICollection<Feedback>? Feedbacks { get; set; }
        public virtual ICollection<ServiceList>? ServiceLists { get; set; }
        public virtual ICollection<ServiceMaterial> ServiceMaterials { get; set; }
    }
}
