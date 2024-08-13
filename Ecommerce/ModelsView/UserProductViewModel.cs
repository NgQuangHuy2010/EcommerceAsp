using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class UserProductViewModel : SearchViewModel
    {

        public List<Category> Categories { get; set; } = new List<Category>();

        public List<Product> Products { get; set; } = new List<Product>();

        public Product? Details { get; set; }
        public List<Product> RelatedProducts { get; set; } = new List<Product>();

    }

}
