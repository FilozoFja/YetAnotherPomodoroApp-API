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
        public PomodoroService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<PomodoroModel> AddPomodoroAsync(AddPomodoroRequest request, int userId)
        {
            var userExist = await _context.Users.AnyAsync(x => x.Id == userId);
            if (!userExist)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found");
            }

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
    }
}
