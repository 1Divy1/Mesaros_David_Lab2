using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Mesaros_David_Lab2.Data;
using Mesaros_David_Lab2.Models;

namespace Mesaros_David_Lab2.Pages.Members
{
    public class DetailsModel : PageModel
    {
        private readonly Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context _context;

        public DetailsModel(Mesaros_David_Lab2.Data.Mesaros_David_Lab2Context context)
        {
            _context = context;
        }

        public Member Member { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Member.FirstOrDefaultAsync(m => m.ID == id);

            if (member is not null)
            {
                Member = member;

                return Page();
            }

            return NotFound();
        }
    }
}
