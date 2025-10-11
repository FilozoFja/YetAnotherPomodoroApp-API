using System.Security.Claims;
using YAPA.Interface;
using YAPA.Models;
using YAPA.Models.Pomodoro;
using YAPA.Models.Response;

namespace YAPA.Handlers;

public class PomodoroHandler
{
    private readonly IPomodoroService _service;
    
    public PomodoroHandler(IPomodoroService service)
    {
        _service = service;
    }

    public async Task<IResult> HandleAddPomodoroAsync(AddPomodoroRequest request, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            throw new UnauthorizedAccessException("User is unauthorized");

        if (!int.TryParse(userId, out int userIdInt))
            throw new UnauthorizedAccessException("Unauthorized access attempt - no user ID in claims");
        

        var result = await _service.AddPomodoroAsync(request, userIdInt);
        var response = new ResponseModel<PomodoroModel>
        {
            Data = result,
            Message = "Pomodoro added successfully",
            Status = true
        };
        return response.Status 
            ? Results.Ok(response) 
            : Results.Problem(
                statusCode: StatusCodes.Status401Unauthorized,
                title: "Unauthorized",
                detail: response.Message
            );
    }

    public async Task<IResult> HandleGetPomodorosByDayAsync(DateTime date, ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            throw new UnauthorizedAccessException("User is unauthorized");

        if (!int.TryParse(userId, out int userIdInt))
            throw new UnauthorizedAccessException("Unauthorized access attempt - no user ID in claims");
        
        var pomodoroByDay = await _service.GetPomodoroByDate(date, userIdInt);
        
        var response = new ResponseModel<PomodoroByDayResponse?>
        {
            Data = pomodoroByDay,
            Message = "Pomodoros retrieved successfully",
            Status = true
        };
        return Results.Ok(response);
    }
}