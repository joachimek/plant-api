using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Species: DbItem
    {
        public long ID { get; set; }
        public string Name { get; set; } = "Unknown plant";
        public string? Info { get; set; }
        public bool IsPublic { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Plants> Plants { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Guides> Guides { get; set; }
    }
}
