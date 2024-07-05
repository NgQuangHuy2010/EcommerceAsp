using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class SearchViewModel
    {
        public string? SearchProduct { get; set; }
        public List<Product> SearchResults { get; set; } = new List<Product>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}
