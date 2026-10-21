using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Mesaros_David_Lab2.Data;
using Mesaros_David_Lab2.Models;

namespace Mesaros_David_Lab2.Pages.Books
{
    public class EditModel : BookCategoriesPageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public EditModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //se va include Author conform cu sarcina de la lab 2
            var book = await _context.Book
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories).ThenInclude(b => b.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

            if (book == null)
            {
                return NotFound();
            }

            Book = book;
            PopulateAssignedCategoryData(_context, Book);

            ViewData["PublisherID"] = new SelectList(
                _context.Publisher, "ID", "PublisherName", Book.PublisherID);
            ViewData["AuthorsID"] = new SelectList(
                _context.Authors.Select(a => new { a.ID, FullName = a.FirstName + " " + a.LastName }),
                "ID",
                "FullName",
                Book.AuthorsID);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id, string[] selectedCategories)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bookToUpdate = await _context.Book
                .Include(book => book.Publisher)
                .Include(book => book.BookCategories)
                    .ThenInclude(category => category.Category)
                .FirstOrDefaultAsync(book => book.ID == id);
            if (bookToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync(
                    bookToUpdate,
                    "Book",
                    book => book.Title,
                    book => book.AuthorsID,
                    book => book.Price,
                    book => book.PublishingDate,
                    book => book.PublisherID))
            {
                UpdateBookCategories(_context, selectedCategories, bookToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            Book = bookToUpdate;
            UpdateBookCategories(_context, selectedCategories, bookToUpdate);
            PopulateAssignedCategoryData(_context, bookToUpdate);

            ViewData["PublisherID"] = new SelectList(
                _context.Publisher, "ID", "PublisherName", Book.PublisherID);
            ViewData["AuthorsID"] = new SelectList(
                _context.Authors.Select(author => new
                {
                    author.ID,
                    FullName = author.FirstName + " " + author.LastName
                }),
                "ID",
                "FullName",
                Book.AuthorsID);
            return Page();
        }
    }
}
