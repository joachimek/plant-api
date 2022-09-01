namespace plant_api.Models.Actions
{
    public class InsertPlantHistRequest
    {
        public long PlantID { get; set; } = -1;
        public bool? Sunlight { get; set; }
        public string? Temperature { get; set; }
        public string? AirHumidity { get; set; }
        public string? SoilHumidity { get; set; }
    }
}
