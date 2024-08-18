namespace Ecommerce.ModelsView
{
    public class PaymentViewModel
    {
        public List<CartItem> Items { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalPayment { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public List<string>? AddressComponents { get; set; } // Optional, if needed
    }
}
