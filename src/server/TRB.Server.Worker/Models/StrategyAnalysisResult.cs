namespace TRB.Server.Worker.Models;

public class StrategyAnalysisResult
{
    public string Market { get; set; } = "";
    public double RSI { get; set; }
    public double MACD { get; set; }
    public DateTime Timestamp { get; set; }
}
