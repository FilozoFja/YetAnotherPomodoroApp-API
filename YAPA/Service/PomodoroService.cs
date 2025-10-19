using Microsoft.EntityFrameworkCore;
using YAPA.Db;
using YAPA.Interface;
using YAPA.Models;
using YAPA.Models.Pomodoro;

namespace YAPA.Service
{
    public class PomodoroService : IPomodoroService
    {
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        public PomodoroService(AppDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        public async Task<PomodoroModel> AddPomodoroAsync(AddPomodoroRequest request, int userId)
        {


            await _authService.CheckIfUserExists(userId);

            var pomodoro = new PomodoroModel
            {
                EndTime = request.EndTime,
                IsCompleted = request.IsCompleted,
                Duration = request.Duration,
                UserId = userId
            };
            _context.Pomodoros.Add(pomodoro);
            await _context.SaveChangesAsync();
            return pomodoro;
        }

        public async Task<PomodoroByDayResponse> GetPomodoroByDate(DateTime date, int userId)
        {
            await _authService.CheckIfUserExists(userId);

            List<PomodoroModel> pomodoro = await _context.Pomodoros
                .Where(p => p.UserId == userId && p.EndTime.Date == date.Date).ToListAsync();
            var pomodoroDataResponseModel = new PomodoroByDayResponse();
            if (pomodoro.Any())
            {
                pomodoroDataResponseModel.TotalDuration = pomodoro.Sum(p => p.Duration);
                pomodoroDataResponseModel.CompletedPomodoros = pomodoro.Count(p => p.IsCompleted);
                pomodoroDataResponseModel.FailedPomodoros = pomodoro.Count(p => !p.IsCompleted);
                pomodoroDataResponseModel.PomodoroDateTimes = pomodoro.Select(p => p.EndTime).ToList();
                pomodoroDataResponseModel.Date = date.Date;
            }
            return pomodoroDataResponseModel;
        }

        public async Task<List<WeeklyPomodoroResponse>> GetWeeklyPomodoro(DateTime dateRangeEnd, int userId)
        {
            await _authService.CheckIfUserExists(userId);
            List<WeeklyPomodoroResponse> weeklyPomodoro = new List<WeeklyPomodoroResponse>();
            var dateRangeStart = dateRangeEnd.AddDays(-7);
            var pomodoros = await _context.Pomodoros
                .Where(x => x.EndTime.Date <= dateRangeEnd.Date 
                            && x.EndTime.Date >= dateRangeStart.Date 
                            && x.UserId == userId)
                .ToListAsync(); 
    
            var grouped = pomodoros
                .GroupBy(x => x.EndTime.Date)
                .Select(g => new WeeklyPomodoroResponse
                {
                    PomodorosDate = g.Key,
                    PomodoroCount = g.Count(),
                    PomodoroFinished = g.Count(p => p.IsCompleted),
                    Duration = g.Sum(p => p.Duration)
                })
                .ToList();
            return grouped;
        }
        
    }
}
