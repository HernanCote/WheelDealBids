namespace IntegrationTests.Utils;

using System.Security.Claims;

public class AuthHelpers
{
    public static Dictionary<string, object> GetBearerForUser(string username)
    {
        return new Dictionary<string, object>
        {
            { ClaimTypes.Name, username }
        };
    }
}