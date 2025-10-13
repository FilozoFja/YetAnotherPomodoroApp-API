using System.Security.Claims;
using YAPA.Interface;
using YAPA.Models;
using YAPA.Models.Pomodoro;
using YAPA.Models.Response;

namespace YAPA.Handlers;

public class PomodoroHandler
{
    private readonly IPomodoroService _pomodoroService;
    private readonly IClaimsService _claimsService;
    
    public PomodoroHandler(IPomodoroService pomodoroService, IClaimsService claimsService)
    {
        _pomodoroService = pomodoroService;
        _claimsService = claimsService;
    }

    public async Task<IResult> HandleAddPomodoroAsync(AddPomodoroRequest request, ClaimsPrincipal user)
    {
        //FLUENTVALIDATION DODAC
        var userIdInt = _claimsService.GetUserIdFromClaims(user);

        var result = await _pomodoroService.AddPomodoroAsync(request, userIdInt);
        var response = new ResponseModel<PomodoroModel>();
        if (result != null)
        {
            response.Data = result;
            response.Message = "Pomodoro added successfully";
            response.Status = true;
        }
        else
        {
            response.Message = "Failed to add pomodoro";
            response.Status = false;
            response.Data = null;
        }
        return Results.Ok(response);
    }

    public async Task<IResult> HandleGetPomodorosByDayAsync(DateTime date, ClaimsPrincipal user)
    {
        //FLUENTVALIDATION DODAC
        var userIdInt = _claimsService.GetUserIdFromClaims(user);

        var result = await _pomodoroService.GetPomodoroByDate(date, userIdInt);
        if(result == null)
            throw new Exception("Failed to retrieve pomodoros");
        
        var response = new ResponseModel<PomodoroByDayResponse?>
        {
            Data = result,
            Message = "Pomodoros retrieved successfully",
            Status = true
        };
        return Results.Ok(response);
    }
}