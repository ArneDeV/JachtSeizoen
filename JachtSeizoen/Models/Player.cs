using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace JachtSeizoen.Models
{
    public class Player
    {
        public string? Name { get; set; }
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        [DataType(DataType.DateTime)]
        public DateTime LastLocTime { get; set; } // Has to be changed to NextLocShown
        // Not needed anymore?
        //public bool Enabled { get; set; }

        public override string ToString() => JsonSerializer.Serialize<Player>(this);
    }
}
