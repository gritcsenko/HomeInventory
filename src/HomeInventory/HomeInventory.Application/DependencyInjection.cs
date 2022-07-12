using HomeInventory.Domain.ValueObjects;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

namespace HomeInventory.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(DependencyInjection).Assembly);
        services.AddTransient<IValueObjectFactory<UserId, Guid>, UserIdFactory>();
        return services;
    }
}
