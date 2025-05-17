using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TRB.Server.Application.Interfaces;
using TRB.Server.Application.Services;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Infrastructure.Repositories;
using TRB.Server.Infrastructure;
using TRB.Server.Infrastructure.Interfaces;
using TRB.Server.Infrastructure.Services;
using TRB.Server.Domain.Options;
using TRB.Server.Presentation.Consumers;
using Microsoft.Extensions.FileProviders;
using TRB.Server.Infrastructure.Messaging;
using TRB.Server.Presistance.Repositories.Commands;
using TRB.Server.Presentation.WebSockets;
using TRB.Server.Presentation.Services;
using StackExchange.Redis;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromFile("NLog.config").GetCurrentClassLogger();
logger.Info("TRB.Server.Presentation Starting...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    });

    builder.Host.UseNLog(); 

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "TRB.Server",
            ValidAudience = "TRB.Client",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ThisIsAVeryStrongSecretKeyForJWT2024!@#$"))
        };
    });

    builder.Services.AddAuthorization();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    builder.Services.AddSingleton<PriceWebSocketHandler>();

    DbConfig.Initialize(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpClient();

    builder.Services.Scan(scan => scan
        .FromAssemblies(
            typeof(UserService).Assembly,
            typeof(UserRepository).Assembly,
            typeof(UpdateUserProfileCommandHandler).Assembly
        )
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repository")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Handler")))
            .AsSelf()
            .WithScopedLifetime()
    );

    builder.Services.AddSingleton<IConnectionMultiplexer>(
        _ => ConnectionMultiplexer.Connect("localhost")
    );
    builder.Services.AddSingleton<IStrategyCacheService, StrategyCacheService>();
    builder.Services.AddSingleton<StrategyAnalyzer>();
    builder.Services.AddSingleton<PriceWebSocketHandler>();
    builder.Services.AddSingleton<IRabbitMQFactory, RabbitMQFactory>();
    builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
    builder.Services.AddSingleton<IRedisService, RedisService>();
    builder.Services.AddScoped<IRedisTokenStore, RedisTokenStore>();
    builder.Services.AddScoped<IRabbitMessagePublisher, RabbitMessagePublisher>();
    builder.Services.AddHostedService<UserSignupConsumerService>();

    var app = builder.Build();
    app.UseWebSockets();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseStaticFiles();
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
        RequestPath = ""
    });

    app.Map("/ws/price", async context =>
    {
        var handler = context.RequestServices.GetRequiredService<PriceWebSocketHandler>();
        await handler.HandleAsync(context);
    });

    app.UseRouting();
    app.UseHttpsRedirection();
    app.UseCors();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "TRB.Server.Presentation failed to start.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
