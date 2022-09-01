namespace plant_api.Models.Plant
{
    public class UpdatePlantRequest
    {
        public long? SpeciesID { get; set; }
        public long? DeviceID { get; set; }
        public string? Name { get; set; }
    }
}
