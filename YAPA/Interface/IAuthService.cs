using YAPA.Models.Auth;

namespace YAPA.Interface;
public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<TokenRefreshResponse> RefreshToken(string refreshToken, string email);
}