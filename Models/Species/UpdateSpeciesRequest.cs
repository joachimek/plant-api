namespace plant_api.Models.Species
{
    public class UpdateSpeciesRequest
    {
        public string? Name { get; set; }
        public string? Info { get; set; }
        public bool? IsPublic { get; set; }
    }
}
