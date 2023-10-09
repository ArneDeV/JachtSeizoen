﻿using JachtSeizoen.Models;
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
            Console.WriteLine(remainingPlayerTime);
            await Clients.Caller.SendAsync("FirstStart", startTimes);
        }

        public async Task RetrieveLocation(string playername, double[] coords)
        {
            Console.WriteLine($"{playername}: Lat={coords[0]}, Lon={coords[1]}");
            jsonFileService.ChangeLoc(playername, coords[1], coords[0]);
            string playerInfo = jsonFileService.GetPlayersString();
            string remainingPlayerTime = GetRevealTime(playername);
            string[] timeComponents = remainingPlayerTime.Split(":");
            int playerTime = int.Parse(timeComponents[0]) * 60 + int.Parse(timeComponents[1]);
            Console.WriteLine(playerTime);
            await Clients.All.SendAsync("LocationUpdate", playerTime, playerInfo, GameSettings!.HunterAmount, GameSettings!.RunnerAmount, playername);
        }

        private string GetRevealTime(string playerName)
        {
            // TODO: Change logic for Next Player time
            Player player = this.jsonFileService.GetPlayer(playerName);
            //DateTime endTime = player.LastLocTime.AddMinutes(GameSettings!.TimeBetween);
            DateTime nextShow = player.NextLocTime;


            Console.WriteLine(nextShow.ToString());


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
