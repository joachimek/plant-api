namespace plant_api.Models
{
    public class PlantsHist : DbItem
    {
        public long ID { get; set; }
        public long PlantID { get; set; }
        public bool Sunlight { get; set; }
        public DateTime Date { get; set; }
        public string Temperature { get; set; }
        public string AirHumidity { get; set; }
        public string SoilHumidity { get; set; }
        public bool WateredPlant { get; set; }
        public bool LampOn { get; set; }
        public bool FanOn { get; set; }
    }
}
