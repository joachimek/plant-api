namespace plant_api.Models.User
{
    public class InsertUserRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string Role { get; set; }
    }
}
