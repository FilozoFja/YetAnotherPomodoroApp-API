using FluentValidation;
using YAPA.Models.Pomodoro;

namespace YAPA.Validators;
public class AddPomodoroRequestValidator : AbstractValidator<AddPomodoroRequest>
{
    public AddPomodoroRequestValidator()
    {
        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("DateTime can not be empty.")
            .Must(DateTimeValidate).WithMessage("Pomodoro should be with max 10 minutes delay.");
        RuleFor(x => x.Duration)
            .NotEmpty().WithMessage("Duration can't be empty.")
            .Must(DurationValidate).WithMessage("Pomodoro should be more than 0 and less than 120 minutes.");
            
    }
    private bool DateTimeValidate(DateTime dateTime)
    {
        var now = DateTime.UtcNow;
        var difference = now - dateTime;
        
        return difference.TotalMinutes >= 0 && difference.TotalMinutes <= 10;
    }

    private bool DurationValidate(int duration)
    {
        return duration < 120 && duration > 0 ? true : false;
    }
}
