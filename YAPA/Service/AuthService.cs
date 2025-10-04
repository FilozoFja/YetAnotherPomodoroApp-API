using YAPA.Interface;
using YAPA.Models.Auth;

namespace YAPA.Service;

public class AuthService : IAuthService
{
    
    public Task<LoginResponse> LoginAsync(LoginRequest request)
    {
    }
}