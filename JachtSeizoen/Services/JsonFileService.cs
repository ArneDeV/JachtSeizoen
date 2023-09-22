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
            Player currentPlayer = players!.First(x => x.Name == playerName);
            if (currentPlayer != null) 
            {
                currentPlayer.LastLocTime = DateTime.Now;
                currentPlayer.Longitude = lon;
                currentPlayer.Latitude = lat;
            }
            Console.WriteLine("Placeholder for changing location");
            Console.WriteLine(players!.First(x => x.Name == playerName).ToString());

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
        }
    }
}
