using AIEngineApi.Services;
using static Microsoft.AspNetCore.Http.StatusCodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// *** Bugslog
// Добавление конфигурации
builder.Configuration.AddEnvironmentVariables();  // брать настройки из окружения

builder.Services.AddHttpClient();
builder.Services.AddScoped<IChatService, ChatService>();
// ***

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
