using JachtSeizoen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JachtSeizoen.Pages
{
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            // Make sure that succes message is off by default
            ViewData["Succes"] = false;
            return Page();
        }

        [BindProperty]
        public Settings? Settings { get; set; }

        // Settings submit
        public IActionResult OnPost()
        {
            // Wrong form --> reset succes form and reload page
            if (!ModelState.IsValid)
            {
                ViewData["Succes"] = false;
                return Page();
            }
            // TODO: Set new settings in the JSON

            // In case of good settings set succes message
            ViewData["Succes"] = true;
            return Page();
        }
    }
}