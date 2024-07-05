namespace Ecommerce.ModelsView
{
    public class CartItem : SearchViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string ImageProductCart { get; set; }
        public decimal ProductPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductPrice * Quantity;

    }
}
