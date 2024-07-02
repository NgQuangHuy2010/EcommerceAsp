namespace Ecommerce.ModelsView
{
    public class CartItem
    {
        public int Id { get; set; } // Sử dụng "Id" thay vì "ProductId"
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity;
    }
}
