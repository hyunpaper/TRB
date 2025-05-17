using TRB.Server.Presentation.Models;

namespace TRB.Server.Infrastructure.Interfaces
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task<bool> ExistsAsync(string key);
        Task<bool> DeleteAsync(string key);

        // 전략 분석 전용 기능
        Task SaveLatestAsync(StrategyAnalysisResultEntity strategy);
        Task SaveToBufferAsync(StrategyAnalysisResultEntity strategy);
    }
}
