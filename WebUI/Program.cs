using Blazored.LocalStorage;
using GarminRunerz.Workout.Services;
using MudBlazor.Services;
using WebUI.Components;
using WebUI.Creators;
using WebUI.Services;
using WebUI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServices();
builder.Services.AddWorkoutServices();
builder.Services.AddScoped<IPlanningLoaderService, PlanningLoaderService>();

// Per-circuit session state
builder.Services.AddScoped<IAthleteSession, AthleteSession>();

// Register all concrete creators (Abstract Factory participants)
builder.Services.AddScoped<IPlannedWorkoutCreator, EasyPlannedWorkoutCreator>();
builder.Services.AddScoped<IPlannedWorkoutCreator, SteadyPlannedWorkoutCreator>();
builder.Services.AddScoped<IPlannedWorkoutCreator, IntervalPlannedWorkoutCreator>();
builder.Services.AddScoped<IPlannedWorkoutCreator, TempoPlannedWorkoutCreator>();
builder.Services.AddScoped<IPlannedWorkoutCreator, LongRunPlannedWorkoutCreator>();
builder.Services.AddScoped<IPlannedWorkoutCreator, RacePlannedWorkoutCreator>();
builder.Services.AddScoped<IPlannedWorkoutCreator, DefaultPlannedWorkoutCreator>();

// Registry factory that picks the right concrete creator
builder.Services.AddScoped<IPlannedWorkoutFactory, PlannedWorkoutFactory>();

builder.Services.AddScoped<IPlanningBuilder, PlanningBuilder>();

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
