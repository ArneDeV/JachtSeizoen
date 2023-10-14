using JachtSeizoen.Models;
using JachtSeizoen.Services;
using Microsoft.AspNetCore.SignalR;
using System.Data;
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

        public async Task RetrieveUpdatedLocation(string playername, double[] coords)
        {
            Console.WriteLine($"{playername}: Lat={coords[0]}, Lon={coords[1]}");
            jsonFileService.ChangeLoc(playername, coords[1], coords[0]);
            string playerInfo = jsonFileService.GetPlayersString();
            string remainingPlayerTime = GetRevealTime(playername);
            string[] timeComponents = remainingPlayerTime.Split(":");
            int playerTime = int.Parse(timeComponents[0]) * 60 + int.Parse(timeComponents[1]);
            await Clients.All.SendAsync("LocationUpdate", playerTime, playerInfo, GameSettings!.HunterAmount, GameSettings!.RunnerAmount, playername);
        }

        // sending location without update
        public async Task RetrieveLocation()
        {
            Console.WriteLine("Forced location");
            string playerInfo = jsonFileService.GetPlayersString();
            await Clients.Caller.SendAsync("LocationForce", playerInfo, GameSettings!.HunterAmount, GameSettings!.RunnerAmount);
        }

        private string GetRevealTime(string playerName)
        {
            Player player = this.jsonFileService.GetPlayer(playerName);
            DateTime nextShow = player.NextLocTime;

            TimeSpan remainingPlayerTime = nextShow.Subtract(value: DateTime.Now);
            Console.WriteLine(remainingPlayerTime.ToString(@"mm\:ss"));
            return remainingPlayerTime.ToString(@"mm\:ss");
        }

        private static string RemaingPlayerTime(Player player)
        {
            DateTime nextShow = player.NextLocTime;
            TimeSpan remainingPlayerTime = nextShow.Subtract(value: DateTime.Now);
            return remainingPlayerTime.ToString(@"mm\:ss");
        }
    }
}
