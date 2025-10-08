using YAPA.Models;
using YAPA.Models.Pomodoro;

namespace YAPA.Interface
{
    public interface IPomodoroService
    {
        Task<PomodoroModel> AddPomodoroAsync(AddPomodoroRequest request, int userId);
    }
}
