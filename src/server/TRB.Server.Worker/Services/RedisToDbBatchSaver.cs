using System.Text.Json;
using StackExchange.Redis;
using TRB.Server.Worker.Models;
using TRB.Server.Worker.Persistence;

namespace TRB.Server.Worker.Services;

public class RedisToDbBatchSaver : BackgroundService
{
    private readonly ILogger<RedisToDbBatchSaver> _logger;
    private readonly IDatabase _redis;
    private readonly IStrategyDatabaseWriter _dbWriter;
    private readonly string[] _markets =
    {
        "KRW-BTC", "KRW-ETH", "KRW-XRP", "KRW-DOGE", "KRW-SOL",
        "KRW-ADA", "KRW-DOT", "KRW-AVAX", "KRW-LINK", "KRW-TRX"
    };

    public RedisToDbBatchSaver(
        ILogger<RedisToDbBatchSaver> logger,
        IConnectionMultiplexer redis,
        IStrategyDatabaseWriter dbWriter)
    {
        _logger = logger;
        _redis = redis.GetDatabase();
        _dbWriter = dbWriter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach (var market in _markets)
                {
                    var key = $"strategy:buffer:{market}";
                    var values = await _redis.ListRangeAsync(key);
                    if (values.Length == 0) continue;

                    var parsed = values
                        .Select(v => JsonSerializer.Deserialize<StrategyAnalysisResultEntity>(v!))
                        .Where(e => e != null)
                        .ToList();

                    if (parsed.Count == 0) continue;

                    var success = await _dbWriter.SaveAsync(parsed);
                    if (success)
                    {
                        await _redis.KeyDeleteAsync(key);
                        _logger.LogInformation(" {Market} 전략 데이터 {Count}건 저장 및 삭제", market, parsed.Count);
                    }
                    else
                    {
                        _logger.LogWarning(" {Market} DB 저장 실패. Redis에 유지됨.", market);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " 배치 저장 작업 실패");
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
