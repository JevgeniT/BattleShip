using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages
{
    public class Settings : PageModel
    {
        [BindProperty] public int Height { get; set; }
        [BindProperty] public int Width { get; set; }
        
        public IActionResult OnGet(int? id)
        {
            return Page();
        }

        public ActionResult OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            return RedirectToPage("./Index");
        }

        
    }
}