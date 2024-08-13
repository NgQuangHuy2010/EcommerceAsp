using Microsoft.AspNetCore.Mvc;
using Ecommerce.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Components
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly EcommerceContext _context;

        public CategoryViewComponent(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Category.ToListAsync();
            return View(categories);
        }

    }
}
