using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.UserManagement.Mapping;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class UserManagementModelMappingsTests : BaseMappingsTests
{
    public UserManagementModelMappingsTests()
    {
        Services.AddDomain();
        Services.AddInfrastructure();
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
        Fixture.CustomizeUlidId<UserId>();
        var sut = CreateSut<UserManagementModelMappings>();
        var instance = Fixture.Create<UserModel>();

        var target = sut.Map<User>(instance);

        target.Id.Value.Should().Be(instance.Id.Value);
        target.Email.Value.Should().Be(instance.Email);
        target.Password.Should().Be(instance.Password);
    }

    [Fact]
    public void ShouldProjectUserModelToUser()
    {
        Fixture.CustomizeUlidId<UserId>();
        var sut = CreateSut<UserManagementContractsMappings, UserManagementModelMappings>();
        var instance = Fixture.Create<UserModel>();
        var source = new[] { instance }.AsQueryable();

        var target = sut.ProjectTo<User>(source, Cancellation.Token).ToArray();

        target.Should().ContainSingle();
    }

    public static TheoryData<object, Type> MapData()
    {
        var fixture = new Fixture();
        fixture.CustomizeUlid();
        fixture.CustomizeUlidId<UserId>();
        fixture.CustomizeEmail();

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
