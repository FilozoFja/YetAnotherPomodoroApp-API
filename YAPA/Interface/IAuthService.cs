using YAPA.Models.Auth;
using YAPA.Models.Response;

namespace YAPA.Interface;
public interface IAuthService
{
    Task<ResponseModel<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ResponseModel<TokenRefreshResponse>> RefreshTokenAsync(string refreshToken, string email);
    Task CheckIfUserExists(int userId);
}