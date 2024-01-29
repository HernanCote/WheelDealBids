namespace UnitTests.Utils;

using System.Security.Claims;

public class Helpers
{
    public static ClaimsPrincipal GetClaimsPrincipal(string name = "test")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, name)
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        
        return new ClaimsPrincipal(identity);
    }
}