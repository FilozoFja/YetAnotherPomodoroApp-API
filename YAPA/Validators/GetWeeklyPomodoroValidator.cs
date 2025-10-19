using FluentValidation;
using YAPA.Models.Pomodoro;

namespace YAPA.Validators;

public class GetWeeklyPomodoroValidator : AbstractValidator<WeeklyPomodoroRequest>
{
    public GetWeeklyPomodoroValidator()
    {
        RuleFor(x => x.EndDateTime)
            .NotEmpty().WithMessage("Date cannot be empty.")
            .Must(BeAValidDate).WithMessage("Invalid date format. Please use yyyy-MM-dd.");
    }
    private bool BeAValidDate(string date)
    {
        return DateTime.TryParse(date, out _);
    }
}