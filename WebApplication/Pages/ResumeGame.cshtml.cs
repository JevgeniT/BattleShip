using System;
using System.Collections.Generic;
using System.Linq;
using BattleShip;
using DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages
{
    public class ResumeGame : PageModel
    {
        private readonly AppDbContext _context;
        [BindProperty] public string GameName { get; set; }
        public List<string> Games { set; get; }

        public ResumeGame(AppDbContext context) { _context = context; }

        public ActionResult OnGet()
        {
            Games  = Config.ListAll();//_context.Records.Select(r => r.FileName).ToList();
            return Page();
        }

        public ActionResult OnPost(string name)
        {
            return Redirect($"./Game?gamename={name}");
        }
    }
}