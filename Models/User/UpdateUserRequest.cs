namespace plant_api.Models.User
{
    public class UpdateUserRequest
    {
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }
    }
}
