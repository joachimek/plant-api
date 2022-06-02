namespace plant_api.Models
{
    public class Reviews: DbItem
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long GuideID { get; set; }
    }
}
