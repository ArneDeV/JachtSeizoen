using System.Text.Json;

namespace JachtSeizoen.Models
{
    public class Player
    {
        public string? Name { get; set; }
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        public DateTime LastLocTime { get; set; }
        // Not needed anymore?
        //public bool Enabled { get; set; }

        public override string ToString() => JsonSerializer.Serialize<Player>(this);
    }
}
