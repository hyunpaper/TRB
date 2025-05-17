using Newtonsoft.Json;
using StackExchange.Redis;
using TRB.Server.Presentation.Models;

namespace TRB.Server.Presentation.Services
{
    public class StrategyCacheService : IStrategyCacheService
    {
        private readonly IDatabase _redis;
        private const string LatestKeyPrefix = "strategy:latest:";
        private const string BufferKeyPrefix = "strategy:buffer:";

        public StrategyCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task SaveLatestAsync(StrategyAnalysisResultEntity strategy)
        {
            var key = $"{LatestKeyPrefix}{strategy.Market}";
            var json = JsonConvert.SerializeObject(strategy, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await _redis.StringSetAsync(key, json);
        }

        public async Task SaveToBufferAsync(StrategyAnalysisResultEntity strategy)
        {
            var key = $"{BufferKeyPrefix}{strategy.Market}";
            var json = JsonConvert.SerializeObject(strategy, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            await _redis.ListRightPushAsync(key, json);
        }

    }
}
