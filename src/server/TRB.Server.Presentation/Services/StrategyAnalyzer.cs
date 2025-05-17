// src/TRB.Server.Presentation/Services/StrategyAnalyzer.cs
using TRB.Server.Presentation.Models;

namespace TRB.Server.Presentation.Services;

public class StrategyAnalyzer
{
    private static readonly Dictionary<string, List<decimal>> _priceHistory = new();
    private const int RsiPeriod = 14;
    private const int ShortEmaPeriod = 12;
    private const int LongEmaPeriod = 26;

    public Task<StrategyAnalysisResultEntity?> AnalyzeAsync(TickerData data)
    {
        if (!_priceHistory.ContainsKey(data.Market))
            _priceHistory[data.Market] = new List<decimal>();

        var prices = _priceHistory[data.Market];
        prices.Add(data.TradePrice);

        if (prices.Count > 100)
            prices.RemoveAt(0);

        if (prices.Count < LongEmaPeriod + 1)
        {
            Console.WriteLine($" {data.Market} 가격 기록 부족으로 분석 생략 (count={prices.Count})");
            return Task.FromResult<StrategyAnalysisResultEntity?>(null);
        }

        var result = new StrategyAnalysisResultEntity
        {
            Market = data.Market,
            Timestamp = data.Timestamp,
            Rsi = CalculateRsi(prices),
            Macd = CalculateMacd(prices),
            Volatility = CalculateVolatility(data),
            AskBidRatio = CalculateAskBidRatio(data),
            ChangeRate = (double)data.SignedChangeRate,
            TradeVolume = (double)data.TradeVolume,
            CreatedAt = DateTime.UtcNow
        };



        return Task.FromResult<StrategyAnalysisResultEntity?>(result);
    }

    private double CalculateRsi(List<decimal> prices)
    {
        if (prices.Count < RsiPeriod + 1) return 50.0;

        decimal gain = 0, loss = 0;
        for (int i = prices.Count - RsiPeriod; i < prices.Count; i++)
        {
            var diff = prices[i] - prices[i - 1];
            if (diff >= 0)
                gain += diff;
            else
                loss -= diff;
        }

        var avgGain = gain / RsiPeriod;
        var avgLoss = loss / RsiPeriod;
        if (avgLoss == 0 && avgGain == 0) return 50.0;
        if (avgLoss == 0) return 100.0;
        if (avgGain == 0) return 0.0;

        var rs = avgGain / avgLoss;
        return (double)(100 - (100 / (1 + rs)));
    }

    private double CalculateMacd(List<decimal> prices)
    {
        var emaShort = CalculateEma(prices, ShortEmaPeriod);
        var emaLong = CalculateEma(prices, LongEmaPeriod);
        return (double)(emaShort - emaLong);
    }

    private decimal CalculateEma(List<decimal> prices, int period)
    {
        if (prices.Count < period)
            return prices.Last();

        var k = 2m / (period + 1);
        decimal ema = prices.Take(period).Average();

        for (int i = period; i < prices.Count; i++)
        {
            ema = (prices[i] - ema) * k + ema;
        }
        return ema;
    }

    private double CalculateVolatility(TickerData data)
    {
        if (data.HighPrice == 0 && data.LowPrice == 0) return 0.0;
        var average = ((double)data.HighPrice + (double)data.LowPrice) / 2;
        return average == 0 ? 0.0 : ((double)(data.HighPrice - data.LowPrice) / average) * 100.0;
    }

    private double CalculateAskBidRatio(TickerData data)
    {
        var total = (double)(data.AccAskVolume + data.AccBidVolume);
        return total == 0 ? 0.5 : (double)data.AccBidVolume / total;
    }
}
