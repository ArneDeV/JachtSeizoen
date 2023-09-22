using JachtSeizoen.Models;
using JachtSeizoen.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace JachtSeizoen.Pages
{
    public class PlayerModel : PageModel
    {
        public PlayerModel(JsonFileService jsonFileService) 
        {
            GameDataService = jsonFileService;
            GameSettings = jsonFileService.GetSettings();
            DateTime endTime = GameSettings!.StartTime.AddMinutes(GameSettings.GameTime);
            TimeSpan RemainingGameTime = endTime.Subtract(value: DateTime.Now);
            GameTimeString = RemainingGameTime.ToString(@"hh\:mm\:ss");
        }

        private JsonFileService GameDataService { get; }

        public Settings? GameSettings { get; }

        [BindProperty(SupportsGet = true)]
        public string? Handler { get; set; }

        public string GameTimeString { get; set; }

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(Handler))
            {
                Console.WriteLine($"Werkt! ({Handler})");
                Console.WriteLine(GameTimeString);
            }
        }
    }
}
