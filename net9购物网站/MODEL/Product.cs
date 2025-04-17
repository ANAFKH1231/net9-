namespace net9购物网站.MODEL
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // New properties
        public int SellerId { get; set; } // Required
        public int? BuyerId { get; set; } // Nullable
    }
}
