using TRB.Server.Application.Interfaces;
using TRB.Server.Application.Services;
using TRB.Server.Domain.Interfaces;
using TRB.Server.Infrastructure.Repositories;
using TRB.Server.Infrastructure;
using TRB.Server.Infrastructure.Interfaces;
using TRB.Server.Infrastructure.Services;
using TRB.Server.Domain.Options;
using TRB.Server.Presentation.Consumers;
using TRB.Server.Presentation.Producers;
using Microsoft.Extensions.FileProviders;


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


// DI µî·Ï
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<UserSignupConsumer>();
builder.Services.AddSingleton<IRabbitMQFactory, RabbitMQFactory>();


builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddScoped<IUserSignupPublisher, UserSignupPublisher>();
builder.Services.AddScoped<UserSignupConsumer>();
builder.Services.AddScoped<IMessagePublisher, RabbitMqPublisher>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await Task.Run(() =>
    {
        using var scope = app.Services.CreateScope();
        var consumer = scope.ServiceProvider.GetRequiredService<UserSignupConsumer>();
        consumer.Start();
    });

}

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    RequestPath = ""
});

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();
app.Run();
