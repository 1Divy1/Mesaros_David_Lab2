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
        public string TitleSort { get; set; } = string.Empty;                                              
        public string AuthorSort { get; set; } = string.Empty;
        public string CurrentSort { get; set; } = string.Empty;
        public string CurrentFilter { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(
            int? id,
            string? sortOrder,
            string? searchString)
        {
            CurrentSort = sortOrder ?? string.Empty;
            CurrentFilter = searchString ?? string.Empty;
            TitleSort = string.IsNullOrEmpty(sortOrder) ? "title_desc" : string.Empty;
            AuthorSort = sortOrder == "author" ? "author_desc" : "author";

            var booksQuery = _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.Authors)
                .Include(b => b.BookCategories)
                    .ThenInclude(bookCategory => bookCategory.Category)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                booksQuery = booksQuery.Where(book =>
                    book.Title.Contains(searchString) ||
                    (book.Authors != null &&
                     (book.Authors.FirstName.Contains(searchString) ||
                      book.Authors.LastName.Contains(searchString))));
            }

            var books = await booksQuery.ToListAsync();
            books = sortOrder switch
            {
                "title_desc" => books.OrderByDescending(book => book.Title).ToList(),
                "author" => books.OrderBy(book => book.Authors == null
                    ? string.Empty
                    : book.Authors.FullName).ToList(),
                "author_desc" => books.OrderByDescending(book => book.Authors == null
                    ? string.Empty
                    : book.Authors.FullName).ToList(),
                _ => books.OrderBy(book => book.Title).ToList()
            };

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
