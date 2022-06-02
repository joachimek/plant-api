namespace plant_api.Models.Actions
{
    public class PlantHistCreate
    {
        public long plantID { get; set; }
        public bool sunlight { get; set; }
        public string temperature { get; set; }
        public string airHumidity { get; set; }
        public string soilHumidity { get; set; }
    }
}
