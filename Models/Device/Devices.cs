using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Devices: DbItem
    {
        public long ID { get; set; }
        public long UserID { get; set; } = -1;
        public long PlantID { get; set; } = -1;
        public string Name { get; set; } = String.Empty;

        [JsonIgnore]
        public virtual Users User { get; set; }
        [JsonIgnore]
        public virtual Plants Plant { get; set; }
    }
}
