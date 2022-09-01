namespace plant_api.Models.Guide
{
    public class InsertGuideRequest
    {
        public long SpeciesID { get; set; } = -1;
        public long UserID { get; set; } = -1;
        public string? Info { get; set; }
        public double? MaxHumidity { get; set; }
        public double? MinHumidity { get; set; }
    }
}
