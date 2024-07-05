using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class HomeViewModel : SearchViewModel  //kế thừa SearchViewModel để dùng các thuộc tính có trong nó
    {
        public List<Category> Category { get; set; } = new List<Category>();
        public List<Product> CategoryProducts { get; set; } = new List<Product>();
        public List<Product> AllProducts { get; set; } = new List<Product>();
        public List<Product> RandomProducts { get; set; } = new List<Product>();


    }

}
