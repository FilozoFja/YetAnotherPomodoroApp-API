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
            var group = app.MapGroup("/pomodoro")
                .WithTags("Pomodoro")
                .WithOpenApi();

            group.MapPost("/add-new",
                    async (AddPomodoroRequest request, ClaimsPrincipal user, IPomodoroService pomodoroService) =>
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
                .WithDescription("Saves user pomodoro to database")
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status201Created)
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status401Unauthorized)
                .RequireRateLimiting("fixed");
            
            app.MapGet("/pomodoro/by-day/{date}", (DateTime date, ClaimsPrincipal user, IPomodoroService pomodoroService) => 
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    throw new UnauthorizedAccessException("User is unauthorized");
                }
                var response = pomodoroService.GetPomodoroByDate(date, int.Parse(userId));
                return Results.Ok(response);
            }).WithName("GetPomodoroByDate")
                .WithSummary("Getting pomodoro by date")
                .WithDescription("Getting pomodoro by date")
                .Produces<ResponseModel<PomodoroModel>>()
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status401Unauthorized)
                .RequireRateLimiting("fixed");
            
            return app;
        }
    }
}