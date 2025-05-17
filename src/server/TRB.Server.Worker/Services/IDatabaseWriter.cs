using TRB.Server.Worker.Models;

namespace TRB.Server.Worker.Services;

public interface IDatabaseWriter
{
    Task SaveAsync(List<StrategyAnalysisResult> data);
}
