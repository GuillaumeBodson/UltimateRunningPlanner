using Blazored.LocalStorage;
using GarminRunerz.Workout.Services;
using MudBlazor.Services;
using WebUI.Components;
using WebUI.DI;
using WebUI.Services;
using WebUI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage(c =>
{
    c.JsonSerializerOptions.WriteIndented = false;
});
builder.Services.AddMudServices();
builder.Services.AddWorkoutServices();
builder.Services.AddScoped<IPlanningLoaderService, PlanningLoaderService>();

// Per-circuit session state
builder.Services.AddScoped<IAthleteSession, AthleteSession>();

// Register all concrete creators (Abstract Factory participants)
builder.Services.AddPlannedWorkoutFactories();

builder.Services.AddScoped<IPlanningBuilder, PlanningBuilder>();

// Register typed client for PaceCalculator.API
builder.Services.AddPaceCalculatorApi(builder.Configuration);

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
