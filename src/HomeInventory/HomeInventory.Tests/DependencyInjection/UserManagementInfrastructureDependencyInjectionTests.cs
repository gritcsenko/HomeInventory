using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Application;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure.Framework;
using HomeInventory.Infrastructure.Persistence;
using MediatR;

namespace HomeInventory.Tests.DependencyInjection;

[UnitTest]
public class UserManagementInfrastructureDependencyInjectionTests : BaseDependencyInjectionTest
{
    public UserManagementInfrastructureDependencyInjectionTests()
    {
        Services.AddSingleton(Substitute.For<IMapper>());
        Services.AddSingleton(Substitute.For<IPublisher>());
        Services.AddSingleton(Substitute.For<IDatabaseContext>());
        Services.AddSingleton(Substitute.For<ISpecificationEvaluator>());
        Services.AddSingleton(Substitute.For<IEventsPersistenceService>());
        AddDateTime();
    }

    [Fact]
    public void ShouldRegister()
    {
        Services.AddUserManagementInfrastructure();
        var provider = CreateProvider();

        Services.Should().ContainSingleScoped<IUserRepository>(provider);
        Services.Should().ContainSingleSingleton<IMappingAssemblySource>(provider);
        Services.Should().ContainSingleSingleton<IPasswordHasher>(provider);
        Services.Should().ContainSingleSingleton<IJsonDerivedTypeInfo>(provider);
        Services.Should().ContainSingleScoped<IDatabaseConfigurationApplier>(provider);
    }
}
