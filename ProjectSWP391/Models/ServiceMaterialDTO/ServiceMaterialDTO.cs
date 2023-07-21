using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ServiceMaterialController.Models.ServiceMaterialDTO
{
    public class ServiceMaterialDTO
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
        public bool ExpiringSoon { get; set; }
        public bool IsExpired
        {
            get
            {
                return ExpiryDate.HasValue && ExpiryDate.Value < DateTime.Today;
            }
        }
        [BindNever]
        public DateTime? CreatedDate { get; set; }

        [BindNever]
        public DateTime? UpdatedDate { get; set; }

        public int ServiceId { get; set; }

        public ProjectSWP391.Models.Service? Service { get; set; }
    }
}
