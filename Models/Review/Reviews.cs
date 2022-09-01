using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Reviews: DbItem
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long GuideID { get; set; }

        [JsonIgnore]
        public virtual Users User { get; set; }
        [JsonIgnore]
        public virtual Guides Guide { get; set; }
    }
}
