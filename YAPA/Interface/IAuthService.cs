using YAPA.Models.Auth;
using YAPA.Models.Response;

namespace YAPA.Interface;
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<TokenRefreshResponse> RefreshTokenAsync(string refreshToken, string email);
    Task CheckIfUserExists(int userId);
}