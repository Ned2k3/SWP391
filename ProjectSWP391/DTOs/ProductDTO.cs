using ProjectSWP391.Models;

namespace ProjectSWP391.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
        public int PcategoryId { get; set; }
        public bool? IsActive { get; set; }

        public ProductCategory? Pcategory { get; set; }
    }
}
