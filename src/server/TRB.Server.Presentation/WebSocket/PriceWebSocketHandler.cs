using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using TRB.Server.Presentation.Models;
using TRB.Server.Presentation.Services;

namespace TRB.Server.Presentation.WebSockets;

public class PriceWebSocketHandler
{
    private readonly ILogger<PriceWebSocketHandler> _logger;
    private readonly StrategyAnalyzer _analyzer; // DI로 주입 받아야 함
    private readonly IStrategyCacheService _strategyCache;
    public PriceWebSocketHandler(ILogger<PriceWebSocketHandler> logger, StrategyAnalyzer analyzer, IStrategyCacheService strategyCache)
    {
        _logger = logger;
        _analyzer = analyzer;
        _strategyCache = strategyCache;
    }

    public async Task HandleAsync(HttpContext context)
    {
        using var socket = await context.WebSockets.AcceptWebSocketAsync();
        var buffer = new byte[4096];

        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                var ticker = JsonConvert.DeserializeObject<TickerData>(json);
                if (ticker == null) continue;

                var strategy = await _analyzer.AnalyzeAsync(ticker);

                if (strategy != null)
                { 
                    _logger.LogInformation(
                        "{Market} 분석 결과: RSI={Rsi}, MACD={Macd}, Volatility={Volatility:F2}%, AskBidRatio={AskBidRatio:P1}, ChangeRate={ChangeRate:P2}, TradeVolume={TradeVolume}, Timestamp={Timestamp}",
                        strategy.Market,
                        strategy.Rsi,
                        strategy.Macd,
                        strategy.Volatility,
                        strategy.AskBidRatio,
                        strategy.ChangeRate,
                        strategy.TradeVolume,
                        strategy.Timestamp
                    );
                    await _strategyCache.SaveAsync(strategy); 

                }

                // TODO: Redis에 저장
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " 파싱/분석 실패: {Json}", json);
            }
        }
    }
}
