using FluentValidation;
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
    private readonly IValidator<AddPomodoroRequest> _addPomodoroRequestValidator;
    private readonly IValidator<GetPomodoroByDayRequest> _getPomodoroByDayRequestValidator;
    public PomodoroHandler(IPomodoroService pomodoroService, IClaimsService claimsService,
        IValidator<AddPomodoroRequest> addPomodoroRequestValidator,
        IValidator<GetPomodoroByDayRequest> getPomodoroByDayRequestValidator)
    {
        _pomodoroService = pomodoroService;
        _claimsService = claimsService;
        _addPomodoroRequestValidator = addPomodoroRequestValidator;
        _getPomodoroByDayRequestValidator =  getPomodoroByDayRequestValidator;
    }

    public async Task<IResult> HandleAddPomodoroAsync(AddPomodoroRequest request, ClaimsPrincipal user)
    {
        var validationResult = _addPomodoroRequestValidator.Validate(request);
        if(!validationResult.IsValid)
            return Results.BadRequest(new ResponseModel<Object>
            {
                Message = "There is a validation issue.",
                Status = false,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        var userIdInt = _claimsService.GetUserIdFromClaims(user);
        var result = await _pomodoroService.AddPomodoroAsync(request, userIdInt);

        return Results.Ok( new ResponseModel<PomodoroModel>
        {
            Data = result,
            Message = "Pomodoro added successfully",
            Status = true
        });
    }

    public async Task<IResult> HandleGetPomodorosByDayAsync(GetPomodoroByDayRequest pomodoroByDayRequest, ClaimsPrincipal user)
    {
        var validationResult = _getPomodoroByDayRequestValidator.Validate(pomodoroByDayRequest);
        if(!validationResult.IsValid)
            return Results.BadRequest(new ResponseModel<Object>
            {
                Message = "There is a validation issue.",
                Status = false,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        
        var userIdInt = _claimsService.GetUserIdFromClaims(user);

        var result = await _pomodoroService.GetPomodoroByDate(pomodoroByDayRequest.Date.ToDateTime(), userIdInt);
        if(result == null)
            throw new Exception("Failed to retrieve pomodoros");

        return Results.Ok(new ResponseModel<PomodoroByDayResponse>
        {
            Data = result,
            Message = "Pomodoros retrieved successfully",
            Status = true
        });

    }
}