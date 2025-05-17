using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using TRB.Server.Infrastructure.Interfaces;


namespace TRB.Server.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;

        public RedisService(IConfiguration configuration)
        {
            var host = configuration["Redis:Host"] ?? "localhost";
            var port = configuration["Redis:Port"] ?? "6379";
            var connection = ConnectionMultiplexer.Connect($"{host}:{port}");
            _db = connection.GetDatabase();
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _db.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _db.KeyExistsAsync(key);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _db.KeyDeleteAsync(key);
        }
    }
}
