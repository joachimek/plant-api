using System.Text.Json.Serialization;

namespace plant_api.Models
{
    public class Users
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<Devices> Devices { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Guides> Guides { get; set; }
        [JsonIgnore]
        public virtual IEnumerable<Reviews> Reviews { get; set; }
    }
}
