namespace YAPA.Models.Auth;

public record LoginResponse(string Token, string Email, int UserId);