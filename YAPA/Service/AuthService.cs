using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using YAPA.Db;
using YAPA.Interface;
using YAPA.Models;
using YAPA.Models.Auth;
using YAPA.Models.Entities;

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
            var refreshToken = await GenerateRefreshToken(user);
            return new LoginResponse(token, refreshToken.Token , user.Email, user.Id);
        }
        throw new UnauthorizedAccessException("Invalid email or password.");
    }

    public async Task<TokenRefreshResponse> RefreshToken(string refreshToken, string email)
    {
        var user = await _context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email == email);
    
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var token = user.RefreshTokens
            .FirstOrDefault(x => 
                x.Token == refreshToken && 
                x.Expires > DateTime.UtcNow &&
                !x.IsRevoked); 
    
        if (token == null)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }
        
        token.IsRevoked = true;
        token.Revoked = DateTime.UtcNow;
        
        var expiredTokens = user.RefreshTokens
            .Where(t => t.Expires < DateTime.UtcNow || t.IsRevoked)
            .ToList();
    
        _context.RefreshTokens.RemoveRange(expiredTokens);
        
        var newJwtToken = _jwtGenerator.JwtGenerator(user);
        var newRefreshToken = await GenerateRefreshToken(user);
    
        await _context.SaveChangesAsync();

        return new TokenRefreshResponse(newJwtToken, newRefreshToken.Token);
    }

    private async Task<RefreshTokenModel> GenerateRefreshToken(UserModel user)
    {
        var refreshToken = new RefreshTokenModel
        {
            Token = Guid.NewGuid().ToString(),
            Expires = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        user.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return refreshToken;
    }
}