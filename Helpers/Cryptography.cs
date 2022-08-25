using System.Security.Cryptography;
using System.Text;

namespace plant_api.Helpers
{
    public static class Cryptography
    {
        public static string MD5Hash(string input)
        {
            MD5 hs = MD5.Create();
            byte[] hash = hs.ComputeHash(Encoding.UTF8.GetBytes(input));
            return Convert.ToBase64String(hash);
        }
    }
}
