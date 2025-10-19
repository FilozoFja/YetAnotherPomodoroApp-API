namespace YAPA.Models.Pomodoro;

public class WeeklyPomodoroResponse
{
    public DateTime PomodorosDate { get; set; }
    public int Duration { get; set; }
    public int PomodoroCount { get; set; }
    public int PomodoroFinished { get; set; }
    public int PomodoroUnfinished { get; set; }
}