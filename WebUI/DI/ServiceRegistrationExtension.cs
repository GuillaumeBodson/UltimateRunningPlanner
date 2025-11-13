using Microsoft.Extensions.Options;
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
}
