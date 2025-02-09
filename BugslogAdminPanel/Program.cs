using BugslogAdminPanel.Components;
using BugslogAdminPanel.Services;
using Radzen;
using static Microsoft.AspNetCore.Http.StatusCodes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// *** Bugslog
builder.Services.AddHttpClient();
builder.Services.AddRadzenComponents();

builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IPromptService, PromptService>();
builder.Services.AddScoped<IAIChatService, AIChatService>();
// ***

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
