using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

namespace HomeInventory.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IUserIdFactory, UserIdFactory>();
        services.AddTransient<IMaterialIdFactory, MaterialIdFactory>();
        services.AddTransient<IProductIdFactory, ProductIdFactory>();
        return services;
    }
}
