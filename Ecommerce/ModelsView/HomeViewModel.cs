﻿using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class HomeViewModel
    {
        public List<Category> Category { get; set; } = new List<Category>();
        public List<Product> Products { get; set; } = new List<Product>();
    }

}
