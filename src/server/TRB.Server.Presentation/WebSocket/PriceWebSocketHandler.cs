// src/TRB.Server.Presentation/WebSockets/PriceWebSocketHandler.cs
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;
using TRB.Server.Presentation.Models;
using TRB.Server.Presentation.Services;

namespace TRB.Server.Presentation.WebSockets;

public class PriceWebSocketHandler
{
    private readonly ILogger<PriceWebSocketHandler> _logger;
    private readonly StrategyAnalyzer _analyzer;
    private readonly IStrategyCacheService _strategyCache;
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, DateTime> _lastGptCall = new();
    private double _totalGptCost = 0; // 총 비용 누적
    private readonly Queue<DateTime> _gptCallTimestamps = new(); // 호출 시간 기록
    private const int GptCallLimitPerMinute = 3;
    private const string GptTargetMarket = "KRW-BTC";
    private DateTime _lastGlobalGptCall = DateTime.MinValue;

    public PriceWebSocketHandler(
        ILogger<PriceWebSocketHandler> logger,
        StrategyAnalyzer analyzer,
        IStrategyCacheService strategyCache)
    {
        _logger = logger;
        _analyzer = analyzer;
        _strategyCache = strategyCache;
        _httpClient = new HttpClient();
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
                    // ✅ GPT 호출 없이 DB 저장만 수행
                    await _strategyCache.SaveLatestAsync(strategy);
                    await _strategyCache.SaveToBufferAsync(strategy);
                    
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

                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " 파싱/분석 실패: {Json}", json);
            }
        }
    }



    /* public async Task HandleAsync(HttpContext context)
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

                     // 최신 전략 상태 저장 (GPT 유무와 무관)
                     await _strategyCache.SaveLatestAsync(strategy);

                     var now = DateTime.UtcNow;
                     var market = strategy.Market;

                     if (strategy.Market == GptTargetMarket && CanCallGpt())
                     {
                         var sw = Stopwatch.StartNew();
                         var gptResult = await CallGptApiAsync(strategy);
                         sw.Stop();

                         if (gptResult != "error")
                         {
                             _lastGlobalGptCall = DateTime.UtcNow;

                             strategy.GptRecommendation = gptResult;
                             await _strategyCache.SaveToBufferAsync(strategy);
                         }
                         else
                         {
                             _logger.LogWarning("GPT 호출 실패. 주기 갱신은 생략됨.");
                         }
                     }
                     else if (strategy.Market != GptTargetMarket)
                     {
                         _logger.LogInformation("📌 마켓 {Market}은 GPT 대상이 아님. 건너뜀", strategy.Market);
                     }
                     else
                     {
                         _logger.LogInformation("⏳ GPT 호출 간격 60초 미만. 요청 생략됨");
                     }
                 }
             }
             catch (Exception ex)
             {
                 _logger.LogError(ex, " 파싱/분석 실패: {Json}", json);
             }
         }
     }*/

    private async Task<string> CallGptApiAsync(StrategyAnalysisResultEntity strategy)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "mykey");



        var body = new
        {
            model = "gpt-3.5-turbo-1106",
            messages = new[]
            {
                new { role = "system", content = "너는 암호화폐 전략 분석을 도와주는 전문가야." },
                new { role = "user", content =
                    $"[{strategy.Market}] 코인에 대해 아래 전략 데이터를 기반으로 매수/매도/유지 중 가장 적절한 행동dmf 매수(BUY), 매도(SELL), 유지(HOLD) 중 하나의 단어만 대답해줘.\n" +
                    $"- RSI: {strategy.Rsi}\n" +
                    $"- MACD: {strategy.Macd}\n" +
                    $"- 변동성: {strategy.Volatility:F2}%\n" +
                    $"- 매수 비율: {strategy.AskBidRatio:P1}\n" +
                    $"- 등락률: {strategy.ChangeRate:P2}\n" +
                    $"- 거래량: {strategy.TradeVolume}"
                }
            },
            max_tokens = 50,
            temperature = 0.7
        };

        var json = JsonConvert.SerializeObject(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning(" GPT API 실패: {StatusCode} - 응답 본문: {ErrorBody}",
                response.StatusCode, errorBody);

            return "error";
        }
        else
        {
            _logger.LogInformation("GPT 연결은 성공입니다");
        }

            var responseContent = await response.Content.ReadAsStringAsync();
        dynamic parsed = JsonConvert.DeserializeObject(responseContent);
        string content = parsed?.choices?[0]?.message?.content;


        int promptTokens = parsed?.usage?.prompt_tokens ?? 0;
        int completionTokens = parsed?.usage?.completion_tokens ?? 0;
        int totalTokens = parsed?.usage?.total_tokens ?? 0;

        double cost = (promptTokens / 1000.0 * 0.0015) + (completionTokens / 1000.0 * 0.002);
        _totalGptCost += cost;

        _logger.LogInformation(" GPT 호출 | Prompt: {Prompt} | Completion: {Completion} | Total: {Total} |  Cost: ${Cost:F6} | 누적 비용: ${TotalCost:F6}",
            promptTokens, completionTokens, totalTokens, cost, _totalGptCost);


        return content?.Trim() ?? "unknown";
    }


    private bool CanCallGpt()
    {
        var now = DateTime.UtcNow;

        // 오래된 기록 제거
        while (_gptCallTimestamps.Count > 0 && (now - _gptCallTimestamps.Peek()).TotalSeconds > 60)
        {
            _gptCallTimestamps.Dequeue();
        }

        return _gptCallTimestamps.Count < GptCallLimitPerMinute;
    }


}
