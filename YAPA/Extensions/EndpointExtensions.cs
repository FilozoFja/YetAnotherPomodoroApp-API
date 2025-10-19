using System.Security.Claims;
using YAPA.Handlers;
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
                    async (AddPomodoroRequest request, ClaimsPrincipal user, PomodoroHandler handler) =>
                        await handler.HandleAddPomodoroAsync(request, user))
                .WithName("AddPomodoro")
                .WithSummary("Adding pomodoro")
                .WithDescription("Saves user pomodoro to database")
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status201Created)
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status400BadRequest)
                .Produces(StatusCodes.Status401Unauthorized)
                .Produces(StatusCodes.Status500InternalServerError)
                .RequireRateLimiting("fixed");
            
            group.MapGet("/by-day",
                    async ([AsParameters] GetPomodoroByDayRequest pomodoroByDayRequest, ClaimsPrincipal user, PomodoroHandler handler) => 
                        await handler.HandleGetPomodorosByDayAsync(pomodoroByDayRequest ,user)
            ).WithName("GetPomodoroByDate")
                .WithSummary("Getting pomodoro by date")
                .WithDescription("Getting pomodoro by date")
                .Produces<ResponseModel<PomodoroModel>>()
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status401Unauthorized)
                .RequireRateLimiting("fixed");
            
            group.MapGet("/by-week",
                async ([AsParameters] WeeklyPomodoroRequest weeklyPomodoroRequest, ClaimsPrincipal user, PomodoroHandler handler) =>
                await handler.HandleGetWeeklyPomodoroRaportAsync(weeklyPomodoroRequest,user)
                ).WithName("GetWeeklyPomodoroRaport")
                .WithSummary("Getting pomodoro by date")
                .WithDescription("Getting pomodoro by date")
                .Produces<ResponseModel<PomodoroModel>>()
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status401Unauthorized)
                .Produces<ResponseModel<PomodoroModel>>(StatusCodes.Status400BadRequest)
                .RequireRateLimiting("fixed");
            
            return app;
        }
    }
}