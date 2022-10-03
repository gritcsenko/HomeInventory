using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Primitives;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Helpers;

internal class ServiceCollectionAssertions : GenericCollectionAssertions<IServiceCollection, ServiceDescriptor, ServiceCollectionAssertions>
{
    public ServiceCollectionAssertions(IServiceCollection value)
        : base(value)
    {
    }

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleTransient<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Transient, provider);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleSingleton<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Singleton, provider);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleScoped<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Scoped, provider);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingle<T>(ServiceLifetime lifetime, IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(lifetime)
            .Which.GetInstance(provider).Should().BeAssignableTo<T>();

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle<T>(ServiceLifetime lifetime)
        where T : class =>
        ContainSingle(typeof(T), lifetime);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleSingleton(Type serviceType) =>
        ContainSingle(serviceType, ServiceLifetime.Singleton);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleton(Type serviceType) =>
        Contain(serviceType, ServiceLifetime.Singleton);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle(Type serviceType, ServiceLifetime lifetime) =>
        ContainSingle(d => d.ServiceType == serviceType && d.Lifetime == lifetime);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> Contain(Type serviceType, ServiceLifetime lifetime) =>
        Contain(d => d.ServiceType == serviceType && d.Lifetime == lifetime);
}
