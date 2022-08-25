namespace plant_api.Models.User
{
    public class DefaultUsers
    {
        public static List<Users> Users = new List<Users>() {
            new Users()
            {
                Username = "admin",
                Password = "admin",
                EmailAddress = "admin@test.pl",
                Role = "Administrator"
            }
        };
    }
}
