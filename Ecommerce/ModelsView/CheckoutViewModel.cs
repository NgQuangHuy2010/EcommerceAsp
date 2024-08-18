using System.ComponentModel.DataAnnotations;
namespace Ecommerce.ModelsView
{
    public class CheckoutViewModel
    {
        public List<CartItem>? Items { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalPayment { get; set; }

        [Required(ErrorMessage = "Vui lòng điền tên của bạn ")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng điền địa chỉ của bạn ")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Vui lòng điền số điện thoại của bạn ")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }
        public List<string>? AddressComponents { get; set; }
    }
}
