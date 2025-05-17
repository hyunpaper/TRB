// src/TRB.Server.Worker/Persistence/StrategyDatabaseWriter.cs
using System.Data;
using MySql.Data.MySqlClient;
using TRB.Server.Worker.Models;
using Microsoft.Extensions.Configuration;

namespace TRB.Server.Worker.Persistence;

public class StrategyDatabaseWriter : BackgroundService, IStrategyDatabaseWriter
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StrategyDatabaseWriter> _logger;

    public StrategyDatabaseWriter(IConfiguration configuration, ILogger<StrategyDatabaseWriter> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SaveAsync(List<StrategyAnalysisResultEntity> results)
    {
        if (results.Count == 0) return true;

        var connStr = _configuration.GetConnectionString("DefaultConnection");

        try
        {
            _logger.LogDebug("DB Save 시작");
            using var conn = new MySqlConnection(connStr);
            await conn.OpenAsync();

            using var tx = await conn.BeginTransactionAsync();

            var values = new List<string>();

            foreach (var r in results)
            {
                values.Add($"('{MySqlHelper.EscapeString(r.Market)}', {r.Timestamp}, {r.Rsi}, {r.Macd}, {r.Volatility}, {r.AskBidRatio}, {r.ChangeRate}, {r.TradeVolume}, '{MySqlHelper.EscapeString(r.GptRecommendation ?? "")}', '{DateTime.Now:yyyy-MM-dd HH:mm:ss}')");
            }

            var sql = $@"
                INSERT INTO strategy_analysis_results
                (market, timestamp, rsi, macd, volatility, ask_bid_ratio, change_rate, trade_volume, gpt_recommendation, created_at)
                VALUES {string.Join(",\n", values)}
                ON DUPLICATE KEY UPDATE
                    rsi = VALUES(rsi),
                    macd = VALUES(macd),
                    volatility = VALUES(volatility),
                    ask_bid_ratio = VALUES(ask_bid_ratio),
                    change_rate = VALUES(change_rate),
                    trade_volume = VALUES(trade_volume),
                    gpt_recommendation = VALUES(gpt_recommendation),
                    created_at = VALUES(created_at);
            ";

            using var command = new MySqlCommand(sql, conn, (MySqlTransaction)tx);
            await command.ExecuteNonQueryAsync();
            await tx.CommitAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, " 전략 데이터 저장 실패");
            return false;
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.CompletedTask;
    }
}
