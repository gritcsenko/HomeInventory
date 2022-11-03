using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services
            .AddGuidIdFactory(id => new UserId(id))
            .AddGuidIdFactory(id => new MaterialId(id))
            .AddGuidIdFactory(id => new ProductId(id))
            .AddGuidIdFactory(id => new StorageAreaId(id));
        services.AddTransient<IEmailFactory, EmailFactory>();
        return services;
    }

    private static IServiceCollection AddGuidIdFactory<TId>(this IServiceCollection services, Func<Guid, TId> createIdFunc)
        where TId : IIdentifierObject<TId>
    {
        services.AddTransient(_ => GuidIdFactory.Create(createIdFunc));
        services.AddTransient<IIdFactory<TId>>(sp => sp.GetRequiredService<IIdFactory<TId, Guid>>());
        return services;
    }
}
