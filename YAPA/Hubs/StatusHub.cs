using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using YAPA.Interface;
using YAPA.Models.Status;
using YAPA.Services;

namespace YAPA.Hubs
{
    public class StatusHub : Hub
    {
        private readonly UserStatusService _statusService;
        private readonly IClaimsService _claimsService;

        public StatusHub(UserStatusService statusService, IClaimsService claimsService)
        {
            _statusService = statusService;
            _claimsService = claimsService;
        }
        
        public override async Task OnConnectedAsync()
        {
            var userId = _claimsService.GetUserIdFromClaims(Context.User);
            
            if (userId > 0)
            {
                await _statusService.SetUserStatusAsync(userId, UserState.Online, TimeSpan.FromMinutes(10));
                await Clients.All.SendAsync("UserStatusChanged", new { userId, state = "Online" });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _claimsService.GetUserIdFromClaims(Context.User);
            if (userId > 0)
            {
                await _statusService.RemoveUserStatusAsync(userId);
                await Clients.All.SendAsync("UserStatusChanged", new { userId, state = "Offline" });
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateStatus(string state, int durationMinutes = 0)
        {
            var userId = _claimsService.GetUserIdFromClaims(Context.User);
            if (userId > 0)
            {
                var parsed = Enum.TryParse<UserState>(state, out var newState)
                    ? newState
                    : UserState.Online;

                var ttl = durationMinutes > 0 ? TimeSpan.FromMinutes(durationMinutes) : (TimeSpan?)null;
                await _statusService.SetUserStatusAsync(userId, parsed, ttl);

                await Clients.All.SendAsync("UserStatusChanged", new { userId, state });
            }
        }
    }
}