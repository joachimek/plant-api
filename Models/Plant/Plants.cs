using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Plants : DbItem
    {
        public long ID { get; set; }
        public long SpeciesID { get; set; } = -1;
        public long GuideID { get; set; } = -1;
        public long DeviceID { get; set; } = -1;
        public string? Name { get; set; }

        [JsonIgnore]
        public virtual SpeciesDto? Species { get; set; }
        [JsonIgnore]
        public virtual Guides? Guide{ get; set; }
        [JsonIgnore]
        public virtual Devices? Device { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<PlantsHist>? PlantsHists { get; set; }
    }
}
