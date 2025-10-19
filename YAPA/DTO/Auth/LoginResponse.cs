namespace YAPA.Models.Auth;

public record LoginResponse(string Token, string RefreshToken, string Email, int UserId);