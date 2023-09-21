using JachtSeizoen.Models;
using JachtSeizoen.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JachtSeizoen.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel(JsonFileService jsonFileService)
        {
            SettingsService = jsonFileService;
            Settings = SettingsService.GetSettings();
        }

        // File service for changing settings
        public JsonFileService SettingsService { get; }

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
            // Set succes message & update setttings
            ViewData["Succes"] = true;
            SettingsService.UpdateSettings(Settings);
            return Page();
        }
    }
}