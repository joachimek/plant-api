using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Guides: DbItem
    {
        public long ID { get; set; }
        public long SpeciesID { get; set; }
        public long UserID { get; set; }
        public string? Info { get; set; }
        public double MaxHumidity { get; set; }

        [JsonIgnore]
        public virtual Species Species { get; set; }
        [JsonIgnore]
        public virtual Users User { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Reviews> Reviews { get; set; }
    }
}
