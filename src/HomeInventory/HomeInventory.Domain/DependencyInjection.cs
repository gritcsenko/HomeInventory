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
            .AddGuidIdFactory(id => new ProductId(id));

        services.AddValueObjectFactory<Email, string, EmailFactory>();
        services.AddSingleton<IAmountFactory, AmountFactory>();
        services.AddSingleton<SystemDateTimeService>();
        services.AddScoped<IDateTimeService>(sp => new FixedDateTimeService(sp.GetRequiredService<SystemDateTimeService>()));
        return services;
    }

    private static IServiceCollection AddGuidIdFactory<TId>(this IServiceCollection services, Func<Guid, TId> createIdFunc)
        where TId : IIdentifierObject<TId>
    {
        services.AddSingleton(createIdFunc);
        services.AddSingleton(sp => GuidIdFactory.CreateFromString(sp.GetRequiredService<Func<Guid, TId>>()));
        services.AddSingleton<IValueObjectFactory<TId, string>>(sp => sp.GetRequiredService<IIdFactory<TId, string>>());

        services.AddSingleton(sp => GuidIdFactory.Create(sp.GetRequiredService<Func<Guid, TId>>()));
        services.AddSingleton<IIdFactory<TId>>(sp => sp.GetRequiredService<IIdFactory<TId, Guid>>());
        services.AddSingleton<IValueObjectFactory<TId, Guid>>(sp => sp.GetRequiredService<IIdFactory<TId, Guid>>());
        return services;
    }

    private static IServiceCollection AddValueObjectFactory<TObject, TValue, TFactory>(this IServiceCollection services)
        where TObject : class, IValueObject<TObject>
        where TFactory : class, IValueObjectFactory<TObject, TValue>
    {
        services.AddSingleton<TFactory>();
        services.AddSingleton<IValueObjectFactory<TObject, TValue>>(sp => sp.GetRequiredService<TFactory>());
        return services;
    }
}
