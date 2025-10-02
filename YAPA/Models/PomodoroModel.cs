namespace YAPA.Models
{
    public class PomodoroModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public int Duration { get; set; }
    }
}
