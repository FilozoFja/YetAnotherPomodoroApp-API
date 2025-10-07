using Microsoft.AspNetCore.Identity.Data;
using YAPA.Interface;

namespace YAPA.Extensions;
public static class AuthEndpointExtensions
{
    public static IEndpointRouteBuilder AuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/login", async (Models.Auth.LoginRequest request, IAuthService authService) =>
        {
            Models.Auth.LoginResponse response = await authService.LoginAsync(request);
            return response;
        }).AllowAnonymous();

        return app;
    }
}

