using System.Security.Claims;
using YAPA.Interface;
using YAPA.Models.Auth;

namespace YAPA.Extensions;
public static class AuthEndpointExtensions
{
    public static IEndpointRouteBuilder AuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            LoginResponse response = await authService.LoginAsync(request);
            return response;
        }).WithName("Login")
        .WithOpenApi()
        .AllowAnonymous();

        app.MapPost("/refresh-token", async (HttpContext httpContext, string refreshToken, string email , IAuthService authService) =>
            {
                TokenRefreshResponse response = await authService.RefreshToken(refreshToken, email);
                return response;
            }).WithName("Refresh-Token")
            .WithOpenApi()
            .AllowAnonymous();

        return app;
    }
}

