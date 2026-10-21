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
    public class CreateModel : BookCategoriesPageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public CreateModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            Book = new Book();
            LoadSelectLists();
            PopulateAssignedCategoryData(_context, Book);
            return Page();
        }

        [BindProperty]
        public Book Book { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync(string[]? selectedCategories)
        {
            var selectedCategoryIds = new HashSet<int>();
            if (selectedCategories != null)
            {
                foreach (var selectedCategory in selectedCategories)
                {
                    if (!int.TryParse(selectedCategory, out var categoryId))
                    {
                        ModelState.AddModelError(string.Empty, "A selected category is invalid.");
                        continue;
                    }

                    selectedCategoryIds.Add(categoryId);
                }
            }

            var validCategoryIds = await _context.Category
                .Where(category => selectedCategoryIds.Contains(category.ID))
                .Select(category => category.ID)
                .ToHashSetAsync();
            if (validCategoryIds.Count != selectedCategoryIds.Count)
            {
                ModelState.AddModelError(string.Empty, "A selected category does not exist.");
            }

            Book.BookCategories = validCategoryIds
                .Select(categoryId => new BookCategory { CategoryID = categoryId })
                .ToList();

            if (!ModelState.IsValid)
            {
                LoadSelectLists();
                PopulateAssignedCategoryData(_context, Book);
                return Page();
            }

            _context.Book.Add(Book);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void LoadSelectLists()
        {
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
        }
    }
}
