using WebUI.Creators;
using WebUI.Services;
using WebUI.Services.Interfaces;

namespace WebUI.DI;

public static class PlannedWorkoutServiceCollectionExtensions
{
    public static IServiceCollection AddPlannedWorkoutFactories(this IServiceCollection services)
    {
        services.AddScoped<IPlannedWorkoutCreator, EasyPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, SteadyPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, IntervalPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, TempoPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, LongRunPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, RacePlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutCreator, DefaultPlannedWorkoutCreator>();
        services.AddScoped<IPlannedWorkoutFactory, PlannedWorkoutFactory>();
        return services;
    }
}