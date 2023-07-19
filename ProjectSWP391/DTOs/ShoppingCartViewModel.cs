namespace ProjectSWP391.DTOs
{
    public class ShoppingCartViewModel
    {
        public List<ShoppingCartModel> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
