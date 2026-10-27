using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mesaros_David_Lab2.Data;
using Mesaros_David_Lab2.Models;
using Mesaros_David_Lab2.ViewModels;

namespace Mesaros_David_Lab2.Pages.Publishers
{
    public class IndexModel : PageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public IndexModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        public PublisherIndexData PublisherData { get; set; } = null!;
        public int? PublisherID { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var publishers = await _context.Publisher
                .Include(publisher => publisher.Books)
                .ThenInclude(book => book.Authors)
                .OrderBy(publisher => publisher.PublisherName)
                .ToListAsync();

            PublisherData = new PublisherIndexData
            {
                Publishers = publishers,
                Books = Array.Empty<Book>()
            };

            if (id.HasValue)
            {
                var publisher = publishers.SingleOrDefault(item => item.ID == id.Value);
                if (publisher is null)
                {
                    return NotFound();
                }

                PublisherID = id;
                PublisherData.Books = publisher.Books ?? Array.Empty<Book>();
            }

            return Page();
        }
    }
}
