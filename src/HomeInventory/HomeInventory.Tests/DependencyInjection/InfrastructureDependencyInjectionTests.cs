using Ardalis.Specification;
using AutoMapper;
using FluentAssertions.Execution;
using HomeInventory.Application;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Mapping;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using HomeInventory.Infrastructure.Persistence.Models.Interceptors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class InfrastructureDependencyInjectionTests : BaseDependencyInjectionTest
{
    public InfrastructureDependencyInjectionTests()
    {
        Services.AddSingleton(Substitute.For<IHostEnvironment>());
        Services.AddSingleton(Substitute.For<IMapper>());
        Services.AddSingleton(Substitute.For<IPublisher>());
        Services.AddSingleton(Substitute.For<IAmountFactory>());
        AddDateTime();
    }

    [Fact]
    public void ShouldRegister()
    {
        Services.AddInfrastructure();
        var provider = CreateProvider();

        using var scope = new AssertionScope();
        Services.Should().ContainSingleScoped<PublishDomainEventsInterceptor>(provider);
        Services.Should().ContainSingleScoped<IUnitOfWork>(provider);
        Services.Should().ContainSingleScoped<IDatabaseContext>(provider);
        Services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
        Services.Should().ContainSingleSingleton<ISpecificationEvaluator>(provider);
        Services.Should().ContainSingleSingleton<AmountObjectConverter>(provider);
        Services.Should().ContainSingleScoped<IEventsPersistenceService>(provider);
        Services.Should().ContainSingleScoped<IDatabaseConfigurationApplier>(provider);
        Services.Should().ContainSingleScoped<PolymorphicDomainEventTypeResolver>(provider);
    }
}
