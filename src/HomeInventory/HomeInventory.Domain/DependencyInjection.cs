using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

namespace HomeInventory.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddValueValidator<IUserIdValidator, UserIdValidator, Guid>();
        services.AddValueObjectFactory<IUserIdFactory, UserIdFactory, UserId, Guid>();
        return services;
    }

    private static IServiceCollection AddValueObjectFactory<TFactory, TObject, TValue>(this IServiceCollection services)
        where TObject : notnull, IValueObject<TObject, TValue>
        where TFactory : class, IValueObjectFactory<TObject, TValue>
    {
        services.AddTransient<TFactory>();
        services.AddTransient<IValueObjectFactory<TObject, TValue>>(sp => sp.GetRequiredService<TFactory>());

        return services;
    }

    private static IServiceCollection AddValueObjectFactory<TInterface, TFactory, TObject, TValue>(this IServiceCollection services)
        where TObject : notnull, IValueObject<TObject, TValue>
        where TInterface : class, IValueObjectFactory<TObject, TValue>
        where TFactory : class, TInterface
    {
        services.AddTransient<TFactory>();
        services.AddTransient<TInterface>(sp => sp.GetRequiredService<TFactory>());
        services.AddTransient<IValueObjectFactory<TObject, TValue>>(sp => sp.GetRequiredService<TInterface>());

        return services;
    }

    private static IServiceCollection AddValueValidator<TInterface, TValidator, TValue>(this IServiceCollection services)
        where TInterface : class, IValueValidator<TValue>
        where TValidator : class, TInterface
    {
        services.AddTransient<TValidator>();
        services.AddTransient<TInterface>(sp => sp.GetRequiredService<TValidator>());
        services.AddTransient<IValueValidator<TValue>>(sp => sp.GetRequiredService<TInterface>());

        return services;
    }
}
