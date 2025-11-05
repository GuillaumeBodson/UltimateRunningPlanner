using Microsoft.Extensions.DependencyInjection;
using Shared.Repository.Abstractions;

namespace Shared.Repository;

public static class ServicesRegistration
{
    public static IServiceCollection AddGenericRepository(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped(typeof(IGenericReadRepository<,>), typeof(GenericReadRepository<,>));
        services.AddScoped(typeof(IGenericWriteRepository<,>), typeof(GenericWriteRepository<,>));
        services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
        return services;
    }
}
