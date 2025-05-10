using TRB.Server.Application.Interfaces;
using TRB.Server.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DI 등록
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 개발 환경에서만 Swagger UI 사용
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
