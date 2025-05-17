using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using TRB.Server.Infrastructure.Interfaces;
using TRB.Server.Presentation.Models;
using TRB.Server.Presentation.Services;


namespace TRB.Server.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;
        private const string BufferKeyPrefix = "strategy:buffer:";
        private const string LatestKeyPrefix = "strategy:latest:";

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
        public async Task SaveLatestAsync(StrategyAnalysisResultEntity strategy)
        {
            if (string.IsNullOrEmpty(strategy.Market))
                throw new ArgumentException("Market 정보는 필수입니다.");

            var key = $"{LatestKeyPrefix}{strategy.Market}";
            var json = JsonConvert.SerializeObject(strategy, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await _db.StringSetAsync(key, json);
        }

        public async Task SaveToBufferAsync(StrategyAnalysisResultEntity strategy)
        {
            if (string.IsNullOrEmpty(strategy.Market))
                throw new ArgumentException("Market 정보는 필수입니다.");
            if (string.IsNullOrEmpty(strategy.GptRecommendation))
                return;

            var key = $"{BufferKeyPrefix}{strategy.Market}";
            var json = JsonConvert.SerializeObject(strategy, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await _db.ListRightPushAsync(key, json);
        }
    }

}
