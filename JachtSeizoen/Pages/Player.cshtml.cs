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
            // General info
            GameDataService = jsonFileService;
            GameSettings = jsonFileService.GetSettings();

            // Game Time data
            DateTime endTime = GameSettings!.StartTime.AddMinutes(GameSettings.GameTime);
            RemainingGameTime = endTime.Subtract(value: DateTime.Now);
        }

        private JsonFileService GameDataService { get; }

        public Settings? GameSettings { get; }

        public Player? CurrentPlayer { get; set; }      

        // Time variables
        public TimeSpan RemainingGameTime { get; set; }
        public string GameTimeString
        {
            get => RemainingGameTime.ToString(@"hh\:mm\:ss");
        }

        public TimeSpan NextShown { get; set; }
        public string ShownString
        {
            get => NextShown.ToString(@"mm\:ss");
        }

        // Argument when loading the page
        [BindProperty(SupportsGet = true)]
        public string? Handler { get; set; }

        public void OnGet()
        {
            if (!string.IsNullOrEmpty(Handler))
            {
                CurrentPlayer = GameDataService.GetPlayer(Handler);
                DateTime showTime = CurrentPlayer.NextLocTime;
                NextShown = showTime.Subtract(value: DateTime.Now);
                Console.WriteLine($"Werkt! ({Handler})");
            }
        }
    }
}
