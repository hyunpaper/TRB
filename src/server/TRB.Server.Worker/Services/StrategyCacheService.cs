using TRB.Server.Worker.Models;

namespace TRB.Server.Worker.Services;

public class StrategyCacheService : IStrategyCacheService
{
    public async Task<List<StrategyAnalysisResult>> GetRecentAsync()
    {
        // TODO: Redis에서 읽기
        return new List<StrategyAnalysisResult>();
    }
}
