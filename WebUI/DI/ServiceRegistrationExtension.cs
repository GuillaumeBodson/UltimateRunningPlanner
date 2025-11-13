using Blazored.LocalStorage;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization.Metadata;
using WebUI.Converters;
using WebUI.Models;
using WebUI.Services;
using WebUI.Services.Interfaces;

namespace WebUI.DI;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddPaceCalculatorApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PaceCalculatorApiOptions>(configuration.GetSection(PaceCalculatorApiOptions.SectionName));

        services.AddHttpClient<IPaceCalculatorClient, PaceCalculatorClient>((sp, http) =>
        {
            var options = sp.GetRequiredService<IOptions<PaceCalculatorApiOptions>>().Value;
            if (string.IsNullOrWhiteSpace(options.BaseUrl))
                throw new InvalidOperationException("PaceCalculatorApi:BaseUrl is not configured.");
            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out var baseUri))
                throw new InvalidOperationException($"PaceCalculatorApi:BaseUrl '{options.BaseUrl}' is not a valid absolute URI.");
            http.BaseAddress = baseUri;
        });

        return services;
    }

    public static IServiceCollection AddSessions(this IServiceCollection services)
    {
        services.AddScoped<ISession<Athlete>, AthleteSession>();
        services.AddScoped<ISession<Planning>, PlanningSession>();

        return services;
    }

    public static IServiceCollection ConfigureLocalStorage(this IServiceCollection services)
    {
        services.AddBlazoredLocalStorage(c =>
        {
            c.JsonSerializerOptions.WriteIndented = false;
            c.JsonSerializerOptions.Converters.Add(new PaceJsonConverter());
            c.JsonSerializerOptions.Converters.Add(new PlannedWorkoutJsonConverter());
            c.JsonSerializerOptions.Converters.Add(new TrainingTemplateCollectionJsonConverter());
            c.JsonSerializerOptions.Converters.Add(new TrainingTemplateConverter());
            c.JsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
           {
                 static typeInfo =>
                 {
                       if (typeInfo.Type == typeof(IWorkoutPreferences))
                       {
                             typeInfo.CreateObject = () => new WorkoutPreferences();
                       }
                 }
           }
            };
        });
        return services;
    }
}
