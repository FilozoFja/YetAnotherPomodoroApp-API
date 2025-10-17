using YAPA.Handlers;
using YAPA.Interface;
using YAPA.Models.Auth;
using YAPA.Models.Response;

namespace YAPA.Extensions;

public static class AuthEndpointExtensions
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/login", 
            async (LoginRequest request, AuthHandler authHandler) =>
                await authHandler.HandleLoginAsync(request))
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticates user and returns JWT and refresh tokens")
            .AllowAnonymous()
            .Produces<ResponseModel<LoginResponse>>()
            .Produces<ResponseModel<LoginResponse>>(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("fixed");

        group.MapPost("/refresh-token", 
            async (TokenRefreshRequest request, AuthHandler authHandler) =>
                await authHandler.HandlerTokenRefresh(request))
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Generates new JWT and refresh tokens using valid refresh token")
            .AllowAnonymous()
            .Produces<ResponseModel<TokenRefreshResponse>>()
            .Produces<ResponseModel<TokenRefreshResponse>>(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("fixed");

        return app;
    }
}