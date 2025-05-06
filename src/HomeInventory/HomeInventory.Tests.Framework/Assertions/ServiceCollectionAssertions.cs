using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Framework.Assertions;

public sealed class ServiceCollectionAssertions(IServiceCollection value, AssertionChain assertionChain) : GenericCollectionAssertions<IServiceCollection, ServiceDescriptor, ServiceCollectionAssertions>(value, assertionChain)
{
    public AndWhichConstraint<ObjectAssertions, IConfigureOptions<TOptions>> ContainConfigureOptions<TOptions>(IServiceProvider provider)
        where TOptions : class =>
        ContainSingle<IConfigureOptions<TOptions>>(provider, ServiceLifetime.Transient);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainConfigureOptions<TOptions>()
        where TOptions : class =>
        ContainSingleTransient<IConfigureOptions<TOptions>>();

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleTransient<T>()
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Transient);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleSingleton<T>()
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Singleton);

    public AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingleScoped<T>()
        where T : class =>
        ContainSingle<T>(ServiceLifetime.Scoped);

    public AndConstraint<ServiceCollectionAssertions> ContainSingleton<T>()
        where T : class =>
        Contain<T>(ServiceLifetime.Singleton);

    public AndConstraint<ServiceCollectionAssertions> ContainTransient<T>()
        where T : class =>
        Contain<T>(ServiceLifetime.Transient);

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
        var matched = Subject.Should().ContainSingle(d => d.ServiceType == serviceType && !d.IsKeyedService && d.Lifetime == lifetime, $"expected single non-keyed {serviceType.FullName} with {nameof(ServiceDescriptor.Lifetime)} = {lifetime}.")
            .Subject;
        return new(this, matched);
    }

    private AndConstraint<ServiceCollectionAssertions> Contain(Type serviceType, ServiceLifetime lifetime)
    {
        Subject.Should().Contain(d => d.ServiceType == serviceType && !d.IsKeyedService && d.Lifetime == lifetime, $"expected non-keyed {serviceType.FullName} with {nameof(ServiceDescriptor.Lifetime)} = {lifetime}.");
        return new(this);
    }

    private AndConstraint<ServiceCollectionAssertions> Contain<T>(ServiceLifetime lifetime)
        where T : class =>
        Contain(typeof(T), lifetime);
}
