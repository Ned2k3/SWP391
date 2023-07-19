﻿namespace ProjectSWP391.DTOs
{
    public class ShoppingCartModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
		public string Image { get; set; }
		public int Quantity { get; set; }
    }

}
