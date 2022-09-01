namespace plant_api.Models.Species
{
    public class InsertSpeciesRequest
    {
        public string Name { get; set; } = "Unknown plant";
        public string? Info { get; set; }
        public bool IsPublic { get; set; }
    }
}
