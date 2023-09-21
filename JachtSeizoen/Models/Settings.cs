﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JachtSeizoen.Models
{
    public class Settings
    {
        // General settings, amount of players for each type
        [Required, Range(1, 3), JsonPropertyName("Jagers")]
        public int HunterAmount { get; set; }

        [Required, Range(1, 2), JsonPropertyName("Lopers")]
        public int RunnerAmount { get; set; }

        // Time that the game runs
        [Required, Range(1, int.MaxValue)]
        public int GameTime { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int TimeBetween { get; set; }

        public override string ToString() => JsonSerializer.Serialize<Settings>(this);
    }
}
