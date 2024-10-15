using HomeInventory.Api;
using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.UserManagement.Mapping;
using HomeInventory.Modules;
using HomeInventory.Web.UserManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class UserManagementModelMappingsTests : BaseMappingsTests
{
    private readonly ModulesCollection _modules = [
        new DomainModule(),
        new LoggingModule(),
        new InfrastructureMappingModule(),
    ];

    public UserManagementModelMappingsTests()
    {
        var builder = Substitute.For<IHostApplicationBuilder>();
        builder.Services.Returns(Services);
#pragma warning disable CA2000 // Dispose objects before losing scope
        builder.Configuration.Returns(new ConfigurationManager());
#pragma warning restore CA2000 // Dispose objects before losing scope
        _modules.InjectTo(builder);

        Fixture.CustomizeId<UserId>();
    }

    [Theory]
    [MemberData(nameof(MapData))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<UserManagementContractsMappings, UserManagementModelMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    [Fact]
    public void ShouldMapUserModelToUser()
    {
        var sut = CreateSut<UserManagementModelMappings>();
        var instance = Fixture.Create<UserModel>();

        var target = sut.Map<User>(instance)!;

        target.Should().NotBeNull();
        target.Id.Value.Should().Be(instance.Id.Value);
        target.Email.Value.Should().Be(instance.Email);
        target.Password.Should().Be(instance.Password);
    }

    [Fact]
    public void ShouldProjectUserModelToUser()
    {
        var sut = CreateSut<UserManagementContractsMappings, UserManagementModelMappings>();
        var instance = Fixture.Create<UserModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.ProjectTo<User>(source, Cancellation.Token).ToArray();

        target.Should().ContainSingle();
    }

    public static TheoryData<object, Type> MapData()
    {
        var timestamp = new DateTimeOffset(new DateOnly(2024, 01, 01), TimeOnly.MinValue, TimeSpan.Zero);
        var fixture = new Fixture();
        fixture.CustomizeId<UserId>(timestamp);
        fixture.CustomizeEmail(timestamp);

        var data = new TheoryData<object, Type>();

        Add<UserId, Ulid>(fixture, data);

        Add<Email, string>(fixture, data);

        Add<User, UserModel>(fixture, data);

        return data;

        static void Add<T1, T2>(IFixture fixture, TheoryData<object, Type> data)
            where T1 : notnull
            where T2 : notnull
        {
            data.Add(fixture.Create<T1>(), typeof(T2));
            data.Add(fixture.Create<T2>(), typeof(T1));
        }
    }
}
