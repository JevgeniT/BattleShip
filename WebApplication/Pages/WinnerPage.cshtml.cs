
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages
{
    public class WinnerPage : PageModel
    {
        [BindProperty] public string Winner { get; set; }
        
        public IActionResult OnGet(string winner)
        {
            Winner = winner;
            return Page();
        }
    }
}