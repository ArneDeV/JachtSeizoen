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
        private string JsonSettings => Path.Combine(WebHostEnvironment.WebRootPath, "data", "jachtseizoentest.json");

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

        public Settings? GetSettings()
        {
            using var jsonFileReader = File.OpenText(JsonSettings);
            return JsonSerializer.Deserialize<Settings>(jsonFileReader.ReadToEnd(),
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }

        // Todo Fix this function
        public void ChangeLoc(string player, double lon, double lat)
        {
            // Might also need time data
            // DateTime currentTime = DateTime.Now;
            Console.WriteLine("Placeholder for changing location");
        }

        public void UpdateSettings(int timeBetw, int gameTime, int hunterAmount, int runnerAmount)
        {
            // Create settings object
            Settings? settings = new Settings { GameTime = gameTime, TimeBetween = timeBetw, HunterAmount = hunterAmount, RunnerAmount = runnerAmount };

            // Write new settings to the settings file
            using var outputStream = File.OpenWrite(JsonSettings);
            JsonSerializer.Serialize<Settings>(
                new Utf8JsonWriter(outputStream, new JsonWriterOptions
                {
                    SkipValidation = true,
                    Indented = true
                }),
                settings
            );
        }
    }
}
