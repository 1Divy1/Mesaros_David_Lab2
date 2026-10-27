using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mesaros_David_Lab2.Data;

namespace Mesaros_David_Lab2.Models
{
    public class IndexModel : PageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public IndexModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        public IList<Category> Category { get; set; } = new List<Category>();
        public IList<Book> Books { get; set; } = new List<Book>();
        public int? CategoryID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Category = await _context.Category
                .Include(category => category.BookCategories)
                    .ThenInclude(bookCategory => bookCategory.Book)
                        .ThenInclude(book => book.Authors)
                .OrderBy(category => category.CategoryName)
                .ToListAsync();

            if (id.HasValue)
            {
                var selectedCategory = Category.SingleOrDefault(category => category.ID == id.Value);
                if (selectedCategory is null)
                {
                    return NotFound();
                }

                CategoryID = selectedCategory.ID;
                Books = selectedCategory.BookCategories?
                    .Select(bookCategory => bookCategory.Book)
                    .OrderBy(book => book.Title)
                    .ToList() ?? new List<Book>();
            }

            return Page();
        }
    }
}
