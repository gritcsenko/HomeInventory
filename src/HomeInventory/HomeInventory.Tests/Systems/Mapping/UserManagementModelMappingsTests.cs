using HomeInventory.Api;
using HomeInventory.Domain;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.UserManagement.Mapping;
using HomeInventory.Infrastructure.UserManagement.Models;
using HomeInventory.Modules;
using HomeInventory.Web.UserManagement;
using Microsoft.Extensions.Configuration;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class UserManagementModelMappingsTests : BaseMappingsTests
{
    private readonly ModulesHost _host = new([new DomainModule(), new LoggingModule(), new InfrastructureMappingModule()]);
    private readonly IConfiguration _configuration = new ConfigurationManager();

    public UserManagementModelMappingsTests()
    {
        Fixture.CustomizeId<UserId>();
        Services.AddSingleton(_configuration);
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await _host.AddModulesAsync(Services, _configuration);
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
        var timestamp = new DateTimeOffset(new(2024, 01, 01), TimeOnly.MinValue, TimeSpan.Zero);
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
