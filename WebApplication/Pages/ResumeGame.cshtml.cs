using System;
using System.Collections.Generic;
using System.Linq;
using DAL;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages
{
    public class ResumeGame : PageModel
    {
        private readonly AppDbContext _context;

        public ResumeGame(AppDbContext context)
        {
            _context = context;
        }

        public List<string> Games { get; set; }
        
        public void OnGet()
        {
            
            Games  = _context.Records.Select(r => r.FileName).ToList();
        }
    }
}