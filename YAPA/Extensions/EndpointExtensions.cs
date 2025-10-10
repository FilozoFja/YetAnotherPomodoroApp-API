using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using StackExchange.Redis;
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
            app.MapPost("/pomodoro", async (AddPomodoroRequest request, ClaimsPrincipal user, IPomodoroService pomodoroService) =>
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
            app.MapGet("/pomodoro/by-day/{date}", (DateTime date, ClaimsPrincipal user, IPomodoroService pomodoroService) => 
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User is unauthorized");
                }
                var response = pomodoroService.GetPomodoroByDate(date, int.Parse(userId));
                return Results.Ok(response);
            });
            
            return app;
        }
    }
}