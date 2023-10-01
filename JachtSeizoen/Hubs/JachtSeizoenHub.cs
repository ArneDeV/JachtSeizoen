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

        public async Task RetrieveLocation(string playername, double[] coords)
        {
            Console.WriteLine($"{playername}: Lat={coords[0]}, Lon={coords[1]}");
            jsonFileService.ChangeLoc(playername, coords[1], coords[0]);
            string playerInfo = jsonFileService.GetPlayersString();
            await Clients.All.SendAsync("LocationUpdate", GameSettings!.TimeBetween * 60, playerInfo, GameSettings!.HunterAmount, GameSettings!.RunnerAmount);
        }

        private string GetRevealTime(string playerName)
        {
            // TODO: Change logic for Next Player time
            Player player = this.jsonFileService.GetPlayer(playerName);
            DateTime endTime = player.LastLocTime.AddMinutes(GameSettings!.TimeBetween);
            TimeSpan remainingPlayerTime = endTime.Subtract(value: DateTime.Now);
            return remainingPlayerTime.ToString(@"mm\:ss");
        }
    }
}
