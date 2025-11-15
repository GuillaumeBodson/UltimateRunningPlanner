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

builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.MaximumReceiveMessageSize = 100 * 1024; // 100 KB
});

builder.Services.ConfigureLocalStorage();

builder.Services.AddMudServices();
builder.Services.AddWorkoutServices();
builder.Services.AddScoped<IPlanningLoaderService, PlanningLoaderService>();
builder.Services.AddScoped<IPaceCalculationService, PaceCalculationService>();

// Per-circuit session state
builder.Services.AddSessions();

// Register all concrete creators (Abstract Factory participants)
builder.Services.AddPlannedWorkoutFactories();

builder.Services.AddScoped<IWorkoutMetadataFactory,  WorkoutMetadataFactory>();

builder.Services.AddScoped<IPlanningBuilder, PlanningBuilder>();

// Prompt builder service
builder.Services.AddScoped<IPlanPromptBuilder, PlanPromptBuilder>();

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
