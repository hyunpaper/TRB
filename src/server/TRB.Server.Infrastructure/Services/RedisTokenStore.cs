using System;
using System.Threading.Tasks;
using TRB.Server.Application.Interfaces;
using TRB.Server.Infrastructure.Interfaces;

namespace TRB.Server.Infrastructure.Services
{
    public class RedisTokenStore : IRedisTokenStore
    {
        private readonly IRedisService _redis;

        public RedisTokenStore(IRedisService redis)
        {
            _redis = redis;
        }

        public Task StoreRefreshTokenAsync(int userId, string token, TimeSpan expires)
        {
            var key = $"refresh:{userId}";
            return _redis.SetAsync(key, token, expires);
        }

        public Task<string?> GetRefreshTokenAsync(int userId)
        {
            var key = $"refresh:{userId}";
            return _redis.GetAsync(key);
        }

        public Task<bool> RemoveRefreshTokenAsync(int userId)
        {
            var key = $"refresh:{userId}";
            return _redis.DeleteAsync(key);
        }
    }
}
