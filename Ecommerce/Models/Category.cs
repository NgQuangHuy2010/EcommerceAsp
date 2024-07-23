using System;
using System.Collections.Generic;

namespace Ecommerce.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Image { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
