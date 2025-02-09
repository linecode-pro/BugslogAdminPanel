using NoSqlBaseApi.Data;
using NoSqlBaseApi.Repositories;
using static Microsoft.AspNetCore.Http.StatusCodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// *** Bugslog
// Добавление конфигурации
builder.Configuration.AddEnvironmentVariables();  // брать настройки из окружения

builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IPromptRepository, PromptRepository>();
// ***

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
