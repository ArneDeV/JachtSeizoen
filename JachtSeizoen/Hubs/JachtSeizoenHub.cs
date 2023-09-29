using JachtSeizoen.Models;
using JachtSeizoen.Services;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using System.Timers;

namespace JachtSeizoen.Hubs
{
    public class JachtSeizoenHub : Hub
    {
        private JsonFileService jsonFileService;

        public JachtSeizoenHub(JsonFileService jsonFileService) : base()
        {
            // GameDataService = jsonFileService;
            this.jsonFileService = jsonFileService;
            GameSettings = jsonFileService.GetSettings();

            // Game Time data
            DateTime endTime = GameSettings!.StartTime.AddMinutes(GameSettings.GameTime);
            RemainingGameTime = endTime.Subtract(value: DateTime.Now);
        }

        private Settings? GameSettings { get; }
        // Time variables
        private TimeSpan RemainingGameTime { get; set; }
        private string GameTimeString
        {
            get => RemainingGameTime.ToString(@"hh\:mm\:ss");
        }

        public async Task RetrieveTimeData(string playerName)
        {
            string remainingPlayerTime = GetRevealTime(playerName);
            string[] startTimes = { GameTimeString, remainingPlayerTime };
            await Clients.Caller.SendAsync("FirstStart", startTimes);
        }

        private string GetRevealTime(string playerName)
        {
            Player player = this.jsonFileService.GetPlayer(playerName);
            DateTime endTime = player.LastLocTime.AddMinutes(GameSettings!.TimeBetween);
            TimeSpan remainingPlayerTime = endTime.Subtract(value: DateTime.Now);
            return remainingPlayerTime.ToString(@"mm\:ss");
        }
    }
}
