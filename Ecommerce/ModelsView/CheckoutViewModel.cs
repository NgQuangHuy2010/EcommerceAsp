namespace Ecommerce.ModelsView
{
    public class CheckoutViewModel
    {
        public List<CartItem> Items { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }
    }
}
