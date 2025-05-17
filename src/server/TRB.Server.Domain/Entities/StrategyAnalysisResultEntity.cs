namespace TRB.Server.Presentation.Models
{
    public class StrategyAnalysisResultEntity
    {
        public string Market { get; set; } = string.Empty;
        public long Timestamp { get; set; }

        public double Rsi { get; set; }
        public double Macd { get; set; }
        public double Volatility { get; set; }
        public double AskBidRatio { get; set; }
        public double ChangeRate { get; set; }
        public double TradeVolume { get; set; }

        public string? GptRecommendation { get; set; } // "매수", "매도", "대기" 등
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
