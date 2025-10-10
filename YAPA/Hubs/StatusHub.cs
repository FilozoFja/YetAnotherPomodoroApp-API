using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using YAPA.Models;
using YAPA.Models.Status;
using YAPA.Services;

namespace YAPA.Hubs
{
    public class StatusHub : Hub
    {
        private readonly UserStatusService _statusService;

        public StatusHub(UserStatusService statusService)
        {
            _statusService = statusService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                await _statusService.SetUserStatusAsync(int.Parse(userId), UserState.Online, TimeSpan.FromMinutes(10));
                await Clients.All.SendAsync("UserStatusChanged", new { userId, state = "Online" });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                await _statusService.RemoveUserStatusAsync(int.Parse(userId));
                await Clients.All.SendAsync("UserStatusChanged", new { userId, state = "Offline" });
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateStatus(string state, int durationMinutes = 0)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                var parsed = Enum.TryParse<UserState>(state, out var newState)
                    ? newState
                    : UserState.Online;

                var ttl = durationMinutes > 0 ? TimeSpan.FromMinutes(durationMinutes) : (TimeSpan?)null;
                await _statusService.SetUserStatusAsync(int.Parse(userId), parsed, ttl);

                await Clients.All.SendAsync("UserStatusChanged", new { userId, state });
            }
        }
    }
}