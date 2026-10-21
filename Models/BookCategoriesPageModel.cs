namespace Mesaros_David_Lab2.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Mesaros_David_Lab2.Data;

public class BookCategoriesPageModel:PageModel
{
    public List<AssignedCategoryData> AssignedCategoryDataList { get; set; } = new();

    public void PopulateAssignedCategoryData(Mesaros_David_Lab2Context context,
        Book book)
    {
        var allCategories = context.Category;
        var bookCategories = book.BookCategories?
            .Select(category => category.CategoryID)
            .ToHashSet() ?? new HashSet<int>();

        AssignedCategoryDataList = new List<AssignedCategoryData>();
        foreach (var cat in allCategories)
        {
            AssignedCategoryDataList.Add(new AssignedCategoryData
            {
                CategoryID = cat.ID,
                Name = cat.CategoryName,
                Assigned = bookCategories.Contains(cat.ID)
            });
        }
    }

    public void UpdateBookCategories(Mesaros_David_Lab2Context context,
        string[]? selectedCategories, Book bookToUpdate)
    {
        var selectedCategoryIds = new HashSet<int>();
        if (selectedCategories != null)
        {
            foreach (var selectedCategory in selectedCategories)
            {
                if (int.TryParse(selectedCategory, out var categoryId))
                {
                    selectedCategoryIds.Add(categoryId);
                }
            }
        }

        var existingCategoryIds = bookToUpdate.BookCategories
            .Select(category => category.CategoryID)
            .ToHashSet();

        foreach (var cat in context.Category)
        {
            if (selectedCategoryIds.Contains(cat.ID))
            {
                if (!existingCategoryIds.Contains(cat.ID))
                {
                    bookToUpdate.BookCategories.Add(new BookCategory
                    {
                        BookID = bookToUpdate.ID,
                        CategoryID = cat.ID
                    });
                }
            }
        }

        foreach (var categoryToRemove in bookToUpdate.BookCategories
                     .Where(category => !selectedCategoryIds.Contains(category.CategoryID))
                     .ToList())
        {
            bookToUpdate.BookCategories.Remove(categoryToRemove);
            context.Remove(categoryToRemove);
        }
    }
}