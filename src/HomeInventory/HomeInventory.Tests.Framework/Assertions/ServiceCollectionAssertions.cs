using HomeInventory.Tests.Framework.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Framework.Assertions;

public sealed class ServiceCollectionAssertions(IServiceCollection value) : GenericCollectionAssertions<IServiceCollection, ServiceDescriptor, ServiceCollectionAssertions>(value)
{
    public AndWhichConstraint<ObjectAssertions, IConfigureOptions<TOptions>> ContainConfigureOptions<TOptions>(IServiceProvider provider)
        where TOptions : class =>
        ContainSingleTransient<IConfigureOptions<TOptions>>(provider);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainConfigureOptions<TOptions>()
        where TOptions : class =>
        ContainSingleTransient<IConfigureOptions<TOptions>>();

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleTransient<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(provider, ServiceLifetime.Transient);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleTransient<T>()
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Transient);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleSingleton<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(provider, ServiceLifetime.Singleton);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleSingleton<T>()
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Singleton);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleScoped<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(provider, ServiceLifetime.Scoped);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleScoped<T>()
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Scoped);

    public AndConstraint<ServiceCollectionAssertions> ContainSingleton<T>(IServiceProvider provider)
        where T : class =>
        Contain<T>(provider, ServiceLifetime.Singleton);

    public AndConstraint<ServiceCollectionAssertions> ContainSingleton<T>()
        where T : class =>
        Contain<T>(ServiceLifetime.Singleton);

    public AndConstraint<ServiceCollectionAssertions> ContainTransient<T>(IServiceProvider provider)
        where T : class =>
        Contain<T>(provider, ServiceLifetime.Transient);

    public AndConstraint<ServiceCollectionAssertions> ContainTransient<T>()
        where T : class =>
        Contain<T>(ServiceLifetime.Transient);

    public AndConstraint<ServiceCollectionAssertions> ContainScoped<T>(IServiceProvider provider)
        where T : class =>
        Contain<T>(provider, ServiceLifetime.Scoped);

    public AndConstraint<ServiceCollectionAssertions> ContainScoped<T>()
        where T : class =>
        Contain<T>(ServiceLifetime.Scoped);

    private AndWhichConstraint<ObjectAssertions, T> ContainSingle<T>(IServiceProvider provider, ServiceLifetime lifetime)
        where T : class =>
        ContainSingle<T>(lifetime)
            .Which.GetInstance(provider).Should().BeAssignableTo<T>();

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle<T>(ServiceLifetime lifetime)
        where T : class =>
        ContainSingle(typeof(T), lifetime);

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle(Type serviceType, ServiceLifetime lifetime)
    {
        var matched = Subject.Where(d => d.ServiceType == serviceType && !d.IsKeyedService)
            .Should().ContainSingle(d => d.Lifetime == lifetime, $"Expected {nameof(ServiceDescriptor.Lifetime)} of the {serviceType.FullName} to be {lifetime}")
            .Subject;
        return new(this, matched);
    }

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> Contain(Type serviceType, ServiceLifetime lifetime)
    {
        return Contain(d => d.ServiceType == serviceType && d.Lifetime == lifetime);
    }

    private AndConstraint<ServiceCollectionAssertions> Contain<T>(IServiceProvider provider, ServiceLifetime lifetime)
        where T : class =>
        Contain<T>(lifetime)
            .And.AllSatisfy(d =>
            {
                if (d.ServiceType == typeof(T) && d.Lifetime == lifetime)
                    d.GetInstance(provider).Should().BeAssignableTo<T>();
            });

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> Contain<T>(ServiceLifetime lifetime)
        where T : class =>
        Contain(typeof(T), lifetime);
}
