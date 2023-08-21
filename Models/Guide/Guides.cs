using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Guides: DbItem
    {
        public long ID { get; set; }
        public long SpeciesID { get; set; } = -1;
        public long UserID { get; set; } = -1;
        public string? Info { get; set; }
        //values of humidity levels and humidity in percentages
        public double MaxHumidity { get; set; } = 100.00;
        public double MinHumidity { get; set; } = 0.00;
        public double AirHumidity { get; set; } = 0.00;
        public double SunlightTime { get; set; } = 0.00;
        public bool IsPublic { get; set; } = false;

        [JsonIgnore]
        public virtual SpeciesDto? Species { get; set; }
        [JsonIgnore]
        public virtual Users? User { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Plants>? Plants { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Reviews>? Reviews { get; set; }
    }
}
