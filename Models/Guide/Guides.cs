namespace plant_api.Models
{
    public class Guides: DbItem
    {
        public long ID { get; set; }
        public long SpeciesID { get; set; }
        public string? Info { get; set; }
        public double MaxHumidity { get; set; }
    }
}
