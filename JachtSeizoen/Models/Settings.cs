using System.ComponentModel.DataAnnotations;

namespace JachtSeizoen.Models
{
    public class Settings
    {
        // General settings, amount of players for each type
        [Required, Range(1, 3)]
        public int HunterAmount { get; set; }

        [Required, Range(1, 2)]
        public int RunnerAmount { get; set; }

        // Time that the game runs
        [Required, Range(1, int.MaxValue)]
        public int GameTime { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int TimeBetween { get; set; }
    }
}
