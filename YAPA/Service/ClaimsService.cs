using System.Security.Claims;
using YAPA.Interface;

namespace YAPA.Service;

public class ClaimsService : IClaimsService
{
    public int GetUserIdFromClaims(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            throw new UnauthorizedAccessException("User is unauthorized");

        if (!int.TryParse(userId, out int userIdInt))
            throw new UnauthorizedAccessException("Unauthorized access attempt - no user ID in claims");
        return userIdInt;
    }
}