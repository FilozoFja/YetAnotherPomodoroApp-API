using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YAPA.Db;
using YAPA.Interface;
using YAPA.Models;
using YAPA.Models.Auth;

namespace YAPA.Service;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserModel> _userManager;
    private readonly IJwtGeneratorService _jwtGenerator;
    public AuthService(AppDbContext context, UserManager<UserModel> userManager, IJwtGeneratorService jwtGenerator)
    {
        _context = context;
        _userManager = userManager;
        _jwtGenerator = jwtGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }
        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (result)
        {
            var token = _jwtGenerator.JwtGenerator(user);
            return new LoginResponse(token, user.Email, user.Id);
        }
        throw new UnauthorizedAccessException("Invalid email or password.");
    }
}