using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class PlantsHist : DbItem
    {
        public long ID { get; set; }
        public long PlantID { get; set; } = -1;
        public DateTime Date { get; set; }
        public bool Sunlight { get; set; } = false;
        public string Temperature { get; set; } = "NaN";
        public string AirHumidity { get; set; } = "NaN";
        public string SoilHumidity { get; set; } = "NaN";
        public bool WateredPlant { get; set; }
        public bool LampOn { get; set; }
        public bool FanOn { get; set; }

        [JsonIgnore]
        public virtual Plants? Plant { get; set; }
    }
}
