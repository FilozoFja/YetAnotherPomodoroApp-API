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
            .LessThan(120).WithMessage("Max duration is 120 minutes.");
        RuleFor(x => x.IsCompleted)
            .NotEmpty().WithMessage("Bool isCompleted can not be empty.");
            
    }
    private bool DateTimeValidate(DateTime dateTime)
    {
        DateTime timeNow = DateTime.UtcNow.AddHours(2);
        var timeDifference = timeNow - dateTime;
        return Math.Abs(timeDifference.Minutes) < 10 ? true : false;
    }
}
