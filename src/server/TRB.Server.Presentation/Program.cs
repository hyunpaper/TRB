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
using Scrutor;


var builder = WebApplication.CreateBuilder(args);

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

DbConfig.Initialize(builder.Configuration);


// DI 등록
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
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRabbitMQFactory, RabbitMQFactory>();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.AddSingleton<IRedisService, RedisService>();
builder.Services.AddScoped<IRedisTokenStore, RedisTokenStore>();
builder.Services.AddScoped<IRabbitMessagePublisher, RabbitMessagePublisher>();
builder.Services.AddHostedService<UserSignupConsumerService>();
builder.Services.AddAuthorization();


var app = builder.Build();

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


app.UseRouting();            // 라우팅 미들웨어
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
