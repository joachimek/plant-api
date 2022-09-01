namespace plant_api.Models.Guide
{
    public class UpdateGuideRequest
    {
        public long? SpeciesID { get; set; }
        public long? UserID { get; set; }
        public string? Info { get; set; }
        public double? MaxHumidity { get; set; }
        public double? MinHumidity { get; set; }
    }
}
