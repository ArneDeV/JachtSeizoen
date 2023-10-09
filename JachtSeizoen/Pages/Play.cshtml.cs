using JachtSeizoen.Models;
using JachtSeizoen.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JachtSeizoen.Pages
{
    public class PlayModel : PageModel
    {
        public PlayModel(JsonFileService jsonFileService)
        {
            GameDataService = jsonFileService;
            Settings? settings = GameDataService.GetSettings();
            // Set hunter amount to the amount in settings, if null than 2
            HunterAmount = settings?.HunterAmount ?? 2;
            RunnerAmount = settings?.RunnerAmount ?? 1;
        }

        private JsonFileService GameDataService { get; }

        public int HunterAmount { get; }
        public int RunnerAmount { get; }

        [BindProperty]
        public string? SelectedBox { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Wrong form --> shouldn't be possible but for safety
            if (!ModelState.IsValid)
            {
                return Page();
            }
            // Temp location placeholder
            //GameDataService.ChangeLoc(SelectedBox!, 50, 50);
            return RedirectToPage("Player",  SelectedBox!);
        }
    }
}
