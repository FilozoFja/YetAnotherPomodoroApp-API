
namespace YAPA.Models.Pomodoro;
public class AddPomodoroRequest
{
    public required DateTime EndTime { get; set; }
    public required bool IsCompleted { get; set; }
    public required int Duration { get; set; }
}
