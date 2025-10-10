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
                //TODO ZROBIC TO JAKO HANDLER DLA WIEKSZEJ WYGODY I POZNIEJSZEJ TESTOWALNOSCI
                //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
                //TODO SPRAWDZANIE JAKOS TEGO EMAIL BO TO MOZE BYC SLABE KIEDY DAPPER WEJDZIE
                if (string.IsNullOrWhiteSpace(request.Email))
                    return Results.BadRequest(new { error = "Email is required" });
                //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
                if (string.IsNullOrWhiteSpace(request.Password))
                    return Results.BadRequest(new { error = "Password is required" });
                
                var response = await authService.LoginAsync(request);
                
                return response.Status
                    ? Results.Ok(response)
                    : Results.Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "Unauthorized",
                        detail: response.Message
                    );
            })
            .WithName("Login")
            .WithSummary("User login")
            .WithDescription("Authenticates user and returns JWT and refresh tokens")
            .AllowAnonymous()
            .Produces<ResponseModel<LoginResponse>>()
            .Produces<ResponseModel<LoginResponse>>(StatusCodes.Status401Unauthorized)
            .RequireRateLimiting("fixed");

        group.MapPost("/refresh-token", async (TokenRefreshRequest request, IAuthService authService) => 
            {
                //TODO ZROBIC TO JAKO HANDLER DLA WIEKSZEJ WYGODY I POZNIEJSZEJ TESTOWALNOSCI
                //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
                //TODO SPRAWDZANIE JAKOS TEGO EMAIL BO TO MOZE BYC SLABE KIEDY DAPPER WEJDZIE
                if (string.IsNullOrWhiteSpace(request.Email))
                    return Results.BadRequest(new { error = "Email is required" });
                //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                    return Results.BadRequest(new { error = "RefreshToken is required" });
                var response = await authService.RefreshTokenAsync(request.RefreshToken, request.Email);

                return response.Status
                    ? Results.Ok(response)
                    : Results.Problem(
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "Unauthorized",
                        detail: response.Message
                    );
            })
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