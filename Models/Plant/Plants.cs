namespace plant_api.Models
{
    public class Plants : DbItem
    {
        public long ID { get; set; }
        public long SpeciesID { get; set; }
        public long GuideID { get; set; } = -1;
        public long DeviceID { get; set; }
        public string? Name { get; set; }
    }
}
