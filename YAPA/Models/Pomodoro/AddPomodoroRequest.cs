using System.ComponentModel.DataAnnotations;

namespace YAPA.Models.Pomodoro;
public class AddPomodoroRequest
{
    [Required]
    [DataType(DataType.DateTime)]
    public required DateTime EndTime { get; set; }
    [Required]
    public required bool IsCompleted { get; set; }
    [Required]
    [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 minutes")]
    public required int Duration { get; set; }
}
