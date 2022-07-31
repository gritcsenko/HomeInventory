using FluentAssertions;
using FluentAssertions.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Helpers;

internal class ServiceCollectionAssertions : GenericCollectionAssertions<IServiceCollection, ServiceDescriptor, ServiceCollectionAssertions>
{
    public ServiceCollectionAssertions(IServiceCollection value)
        : base(value)
    {
    }

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleTransient<T>()
        where T : class
    {
        return ContainSingle<T>(ServiceLifetime.Transient);
    }

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleSingleton<T>()
        where T : class
    {
        return ContainSingle<T>(ServiceLifetime.Singleton);
    }

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleScoped<T>()
        where T : class
    {
        return ContainSingle<T>(ServiceLifetime.Scoped);
    }

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle<T>(ServiceLifetime lifetime)
        where T : class
    {
        return ContainSingle(typeof(T), lifetime);
    }

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleSingleton(Type serviceType)
    {
        return ContainSingle(serviceType, ServiceLifetime.Singleton);
    }

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle(Type serviceType, ServiceLifetime lifetime)
    {
        return ContainSingle(d => d.ServiceType == serviceType && d.Lifetime == lifetime);
    }
}
