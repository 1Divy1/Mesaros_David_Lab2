using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mesaros_David_Lab2.Data;
using Mesaros_David_Lab2.Models;

namespace Mesaros_David_Lab2.Pages.Books
{
    public class IndexModel : PageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public IndexModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        public BookData BookD { get; set; } = new();
        public int BookID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var books = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.BookCategories)
                    .ThenInclude(bookCategory => bookCategory.Category)
                .AsNoTracking()
                .OrderBy(book => book.Title)
                .ToListAsync();

            BookD = new BookData
            {
                Books = books,
                Categories = await _context.Category.AsNoTracking().ToListAsync(),
                BookCategories = books.SelectMany(book => book.BookCategories)
            };

            if (id is not null)
            {
                var selectedBook = books.SingleOrDefault(book => book.ID == id.Value);
                if (selectedBook is null)
                {
                    return NotFound();
                }

                BookID = selectedBook.ID;
                BookD.Categories = selectedBook.BookCategories
                    .Select(bookCategory => bookCategory.Category)
                    .ToList();
            }

            return Page();
        }
    }
}
