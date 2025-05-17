using TRB.Server.Presentation.Models;

namespace TRB.Server.Presentation.Services
{
    public interface IStrategyCacheService
    {
        /// <summary>
        /// 최신 전략 결과를 저장 (GPT 여부와 무관)
        /// </summary>
        Task SaveLatestAsync(StrategyAnalysisResultEntity strategy);

        /// <summary>
        /// GPT 포함된 전략 결과를 버퍼에 저장 (배치용)
        /// </summary>
        Task SaveToBufferAsync(StrategyAnalysisResultEntity strategy);
    }
}
