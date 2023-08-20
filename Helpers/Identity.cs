using System.Security.Claims;

namespace plant_api.Helpers
{
    public static class Identity
    {
        public static long GetUserId(ClaimsIdentity identity)
        {
            if (long.TryParse(identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value, out var userId))
            {
                return userId;

            }
            throw new Exception("userId not found");
        }
    }
}
