using System.Security.Claims;
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

        group.MapPost("/login", async (LoginRequest request, IAuthService authService) =>
        {
            var response = await authService.LoginAsync(request);

            return response.Status
                ? Results.Ok(response)
                : Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
        })
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticates user and returns JWT and refresh tokens")
            .AllowAnonymous()
            .Produces<ResponseModel<LoginResponse>>(StatusCodes.Status200OK)
            .Produces<ResponseModel<LoginResponse>>(StatusCodes.Status401Unauthorized);

        group.MapPost("/refresh-token", async (TokenRefreshRequest request, IAuthService authService) =>
        {
            var response = await authService.RefreshTokenAsync(request.RefreshToken, request.Email);

            return response.Status
                ? Results.Ok(response)
                : Results.Json(response, statusCode: StatusCodes.Status401Unauthorized);
        })
            .WithName("RefreshToken")
            .WithSummary("Refresh access token")
            .WithDescription("Generates new JWT and refresh tokens using valid refresh token")
            .AllowAnonymous()
            .Produces<ResponseModel<TokenRefreshResponse>>(StatusCodes.Status200OK)
            .Produces<ResponseModel<TokenRefreshResponse>>(StatusCodes.Status401Unauthorized);

        return app;
    }
}