using TRB.Server.Worker.Models;

namespace TRB.Server.Worker.Services;

public interface IStrategyCacheService
{
    Task<List<StrategyAnalysisResult>> GetRecentAsync();
}
