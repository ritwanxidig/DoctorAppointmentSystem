

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Helpers;
public static class PricipalUtilities
{
    public static int GetId(this ClaimsPrincipal principal)
    {
        var input = principal.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if(string.IsNullOrEmpty(input))
        {
            input = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        }
        int.TryParse(input, out var UserId);
        return UserId;
    }

    public static int getDoctorId(this ClaimsPrincipal principal)
    {
        int.TryParse(principal.FindFirstValue("doctorId"), out var doctorId);
        return doctorId;
    }
}