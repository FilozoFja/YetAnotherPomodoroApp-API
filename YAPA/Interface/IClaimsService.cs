using System.Security.Claims;

namespace YAPA.Interface;

public interface IClaimsService
{
 int GetUserIdFromClaims(ClaimsPrincipal user);   
}