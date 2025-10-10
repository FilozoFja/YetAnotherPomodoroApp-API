using StackExchange.Redis;
using System.Text.Json;
using YAPA.Models;
using YAPA.Models.Status;

namespace YAPA.Services
{
    public class UserStatusService
    {
        private readonly IDatabase _redis;

        public UserStatusService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task SetUserStatusAsync(int userId, UserState state, TimeSpan? ttl = null)
        {
            var key = $"user:{userId}:status";
            var value = JsonSerializer.Serialize(new
            {
                state = state.ToString(),
                updated = DateTime.UtcNow
            });

            await _redis.StringSetAsync(key, value, ttl);
        }

        public async Task<string?> GetUserStatusAsync(int userId)
        {
            var key = $"user:{userId}:status";
            var value = await _redis.StringGetAsync(key);
            return value.HasValue ? value.ToString() : null;
        }

        public async Task RemoveUserStatusAsync(int userId)
        {
            await _redis.KeyDeleteAsync($"user:{userId}:status");
        }

        public IEnumerable<RedisKey> GetAllKeys()
        {
            var server = ((ConnectionMultiplexer)_redis.Multiplexer).GetServer("localhost", 6379);
            return server.Keys(pattern: "user:*:status");
        }
    }
}