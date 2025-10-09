using YAPA.Models;
using YAPA.Models.Pomodoro;
using YAPA.Models.Response;

namespace YAPA.Interface
{
    public interface IPomodoroService
    {
        Task<PomodoroModel> AddPomodoroAsync(AddPomodoroRequest request, int userId);
        Task<ResponseModel<PomodoroByDayResponse>> GetPomodoroByDate(DateTime date, int userId);

    }
}
