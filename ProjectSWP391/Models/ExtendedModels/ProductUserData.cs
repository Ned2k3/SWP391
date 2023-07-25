namespace ProjectSWP391.Models.ExtendedModels
{
    public class ProductUserData
    {
        public string BuyerEmail { get; set; }
        public string ProductName { get; set; }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int Amount { get; set; }
        public string? Content { get; set; }
        public decimal Total { get; set; }

        public ProductUserData(string buyerEmail, string productName,int orderId, DateTime orderDate, int amount, string? content,decimal total)
        {
            BuyerEmail = buyerEmail;
            ProductName = productName;
            OrderId = orderId;
            OrderDate = orderDate;
            Amount = amount;
            Content = content;
            Total = total;
        }
    }
}
