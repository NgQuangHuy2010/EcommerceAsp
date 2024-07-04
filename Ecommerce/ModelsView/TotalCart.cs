using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class TotalCart
    {
        public List<CartItem> Items { get; set; }

        public List<Product> image { get; set; } = new List<Product>();
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    }
}
