namespace YAPA.Models.Pomodoro
{
    public class PomodoroByDayResponse
    {
        public DateTime Date { get; set; }
        public int FailedPomodoros { get; set; }
        public int CompletedPomodoros { get; set; }
        public int TotalDuration { get; set; }
        public List<DateTime> PomodoroDateTimes { get; set; } = new List<DateTime>();
    }
}
