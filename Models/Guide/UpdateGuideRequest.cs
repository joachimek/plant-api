namespace plant_api.Models.Guide
{
    public class UpdateGuideRequest
    {
        public string? Info { get; set; }
        public double? MaxHumidity { get; set; }
        public double? MinHumidity { get; set; }
        public double? AirHumidity { get; set; }
        public double? SunlightTime { get; set; }
        public bool? IsPublic { get; set; }
    }
}
