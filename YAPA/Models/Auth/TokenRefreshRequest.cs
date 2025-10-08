namespace YAPA.Models.Auth;
public record TokenRefreshRequest(string RefreshToken, string Email);
