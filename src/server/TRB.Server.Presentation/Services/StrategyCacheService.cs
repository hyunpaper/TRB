using Newtonsoft.Json;
using StackExchange.Redis;
using TRB.Server.Presentation.Models;

namespace TRB.Server.Presentation.Services
{
    public class StrategyCacheService : IStrategyCacheService
    {
        private readonly IDatabase _redis;

        public StrategyCacheService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        public async Task SaveAsync(StrategyAnalysisResultEntity result)
        {
            var key = $"strategy:buffer:{result.Market}";
            var json = JsonConvert.SerializeObject(result);
            await _redis.ListRightPushAsync(key, json);
        }
    }
}
