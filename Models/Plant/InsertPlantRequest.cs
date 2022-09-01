namespace plant_api.Models.Plant
{
    public class InsertPlantRequest
    {
        public long SpeciesID { get; set; } = -1;
        public long GuideID { get; set; } = -1;
        public long DeviceID { get; set; } = -1;
        public string? Name { get; set; }
    }
}
