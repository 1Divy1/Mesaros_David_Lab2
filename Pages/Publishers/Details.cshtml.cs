using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mesaros_David_Lab2.Data;
using Mesaros_David_Lab2.Models;

namespace Mesaros_David_Lab2.Pages.Publishers
{
    public class DetailsModel : PageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public DetailsModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        public Publisher Publisher { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var publisher = await _context.Publisher.FirstOrDefaultAsync(m => m.ID == id);

            if (publisher is not null)
            {
                Publisher = publisher;

                return Page();
            }

            return NotFound();
        }
    }
}
