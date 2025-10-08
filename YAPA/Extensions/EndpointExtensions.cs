using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using YAPA.Interface;
using YAPA.Models;
using YAPA.Models.Pomodoro;
using YAPA.Models.Response;

namespace YAPA.Extensions
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder PomodoroEndpoints( this IEndpointRouteBuilder app )
        {
            app.MapPost("/add-pomodoro", async (AddPomodoroRequest request, ClaimsPrincipal user, IPomodoroService pomodoroService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return Results.Unauthorized();
                }
                var result = await pomodoroService.AddPomodoroAsync(request, int.Parse(userId));
                var response = new ResponseModel<PomodoroModel>
                {
                    Data = result,
                    Message = "Pomodoro added successfully",
                    Status = true
                };
                return Results.Ok(response);

            }).WithName("AddPomodoro")
            .WithSummary("Adding pomodoro")
            .WithDescription("Saves user pomodoro to database");

            app.MapGet("/whoami", (ClaimsPrincipal user) =>
            {
                var claims = user.Claims.Select(c => new { c.Type, c.Value });
                return Results.Json(claims);
            }).RequireAuthorization();

            return app;
        }
    }
}