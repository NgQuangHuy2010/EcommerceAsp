using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class TotalCart : SearchViewModel
    {
        public List<CartItem> Items { get; set; }

        public List<Product> Image { get; set; } = new List<Product>();
        public decimal TotalAmount => Items.Sum(item => item.TotalPrice);
    }


}
