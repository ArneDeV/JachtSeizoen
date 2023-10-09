using JachtSeizoen.Models;
using System.Text.Json;

namespace JachtSeizoen.Services
{
    public class JsonFileService
    {
        public JsonFileService(IWebHostEnvironment webHostEnvironment)
        {
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }

        // Files that store players and game settings
        private string JsonPlayers => Path.Combine(WebHostEnvironment.WebRootPath, "data", "players.json");
        private string JsonSettings => Path.Combine(WebHostEnvironment.WebRootPath, "data", "settings.json");

        // Get the players --> used in change Loc
        public IEnumerable<Player>? GetPlayers()
        {
            using var jsonFileReader = File.OpenText(JsonPlayers);
            return JsonSerializer.Deserialize<Player[]>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        public string GetPlayersString()
        {
            using var jsonFileReader = File.OpenText(JsonPlayers);
            return jsonFileReader.ReadToEnd();
        }

        public Player GetPlayer(string playerName)
        {
            IEnumerable<Player>? players = GetPlayers();
            return players!.First(x => x.Name == playerName);
        }

        public Settings? GetSettings()
        {
            using var jsonFileReader = File.OpenText(JsonSettings);
            return JsonSerializer.Deserialize<Settings>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        // TODO Fix this function
        public void ChangeLoc(string playerName, double lon, double lat)
        {
            // Get players
            IEnumerable<Player>? players = GetPlayers();
            Settings settings = GetSettings()!;
            DateTime current = DateTime.Now;
            Player currentPlayer = players!.First(x => x.Name == playerName);
            if (currentPlayer != null) 
            {   
                currentPlayer.NextLocTime = current.AddSeconds(settings.TimeBetween * 60);
                currentPlayer.Longitude = lon;
                currentPlayer.Latitude = lat;
            }
            // First player list
            File.Create(JsonPlayers).Close();
            // Add the new playerInfo
            using var outputStream = File.OpenWrite(JsonPlayers);
            JsonSerializer.Serialize<IEnumerable<Player>>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                players!
            );
            outputStream.Close();
        }

        // Write new settings to the settings file
        public void UpdateSettings(Settings? newSettings)
        {
            // First clear settings
            File.Create(JsonSettings).Close();
            // Add the new settings
            using var outputStream = File.OpenWrite(JsonSettings);
            JsonSerializer.Serialize<Settings>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                newSettings!
            );
            outputStream.Close();
            // Start the player timers
            StartTimers();
        }

        public void StartTimers()
        {
            IEnumerable<Player>? players = GetPlayers();
            Settings settings = GetSettings()!;
            foreach(Player player in players!)
            {
                //player.LastLocTime = DateTime.Now;
                player.NextLocTime = DateTime.Now.AddSeconds(settings.TimeBetween*60);
                player.Latitude = 51.06678;
                player.Longitude = 3.630376;
            }
            // First remove player list
            File.Create(JsonPlayers).Close();
            using var outputStream = File.OpenWrite(JsonPlayers);
            JsonSerializer.Serialize<IEnumerable<Player>>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                players
            );
            outputStream.Close();
        }
    }
}
