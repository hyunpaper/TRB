using TRB.Server.Application.Interfaces;
using TRB.Server.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// DI ���
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ���� ȯ�濡���� Swagger UI ���
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
