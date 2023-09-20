namespace JachtSeizoen.Models
{
    public class Jager
    {
        public double Longitude { get; set; } = 0;
        public double Latitude { get; set; } = 0;
        public bool Enabled { get; set; }
        public DateTime LastLocTime { get; set; }
    }
}
