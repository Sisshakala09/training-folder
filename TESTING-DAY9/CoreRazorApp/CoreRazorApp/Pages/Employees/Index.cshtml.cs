using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CoreRazorApp.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly OrgContext _context;

        public IndexModel(OrgContext context)
        {
            _context = context;
        }

        public IList<Employee> Employee { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Employee = await _context.employees
                .Include(e => e.dept).ToListAsync();
        }
    }
}
