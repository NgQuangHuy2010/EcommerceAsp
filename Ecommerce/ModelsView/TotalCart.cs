﻿using Ecommerce.Models;

namespace Ecommerce.ModelsView
{
    public class TotalCart : SearchViewModel
    {

        public List<Product> Image { get; set; } = new List<Product>();

        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public decimal TotalAmount => Items?.Sum(item => item.TotalPrice) ?? 0;
    }


}
