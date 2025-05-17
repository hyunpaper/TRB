using TRB.Server.Presentation.Models;

namespace TRB.Server.Presentation.Services
{
    public interface IStrategyCacheService
    {
        Task SaveAsync(StrategyAnalysisResultEntity result);
    }
}
