using HomeInventory.Tests.Framework.Assertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HomeInventory.Tests.Framework.Assertions;

public class ServiceCollectionAssertions : GenericCollectionAssertions<IServiceCollection, ServiceDescriptor, ServiceCollectionAssertions>
{
    public ServiceCollectionAssertions(IServiceCollection value)
        : base(value)
    {
    }

    public AndWhichConstraint<ObjectAssertions, IConfigureOptions<TOptions>> ContainConfigureOptions<TOptions>(IServiceProvider provider)
        where TOptions : class =>
        ContainSingleTransient<IConfigureOptions<TOptions>>(provider);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleTransient<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(provider, ServiceLifetime.Transient);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleSingleton<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(provider, ServiceLifetime.Singleton);

    public AndWhichConstraint<ObjectAssertions, T> ContainSingleScoped<T>(IServiceProvider provider)
        where T : class =>
        ContainSingle<T>(provider, ServiceLifetime.Scoped);

    public AndConstraint<ServiceCollectionAssertions> ContainTransient<T>(IServiceProvider provider)
        where T : class =>
        Contain<T>(provider, ServiceLifetime.Transient);

    private AndWhichConstraint<ObjectAssertions, T> ContainSingle<T>(IServiceProvider provider, ServiceLifetime lifetime)
        where T : class =>
        ContainSingle<T>(lifetime)
            .Which.GetInstance(provider).Should().BeAssignableTo<T>();

    private AndConstraint<ServiceCollectionAssertions> Contain<T>(IServiceProvider provider, ServiceLifetime lifetime)
        where T : class =>
        Contain<T>(lifetime)
            .And.AllSatisfy(d =>
            {
                if (d.ServiceType == typeof(T) && d.Lifetime == lifetime)
                    d.GetInstance(provider).Should().BeAssignableTo<T>();
            });

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle<T>(ServiceLifetime lifetime)
        where T : class =>
        ContainSingle(typeof(T), lifetime);

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> Contain<T>(ServiceLifetime lifetime)
        where T : class =>
        Contain(typeof(T), lifetime);

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> ContainSingle(Type serviceType, ServiceLifetime lifetime) =>
        ContainSingle(d => d.ServiceType == serviceType && d.Lifetime == lifetime);

    private AndWhichConstraint<ServiceCollectionAssertions, ServiceDescriptor> Contain(Type serviceType, ServiceLifetime lifetime) =>
        Contain(d => d.ServiceType == serviceType && d.Lifetime == lifetime);
}
