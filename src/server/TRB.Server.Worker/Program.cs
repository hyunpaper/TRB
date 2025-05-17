using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TRB.Server.Worker.Services;
using StackExchange.Redis;
using TRB.Server.Worker.Persistence;
using NLog;

LogManager.Setup().LoadConfigurationFromFile("../TRB.LoggingConfig/NLog.config");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<RedisToDbBatchSaver>();

builder.Services.AddSingleton<IStrategyCacheService, StrategyCacheService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect("localhost:6379")
);

builder.Services.AddSingleton<IStrategyDatabaseWriter, StrategyDatabaseWriter>();
builder.Services.AddHostedService<RedisToDbBatchSaver>();


await builder.Build().RunAsync();
