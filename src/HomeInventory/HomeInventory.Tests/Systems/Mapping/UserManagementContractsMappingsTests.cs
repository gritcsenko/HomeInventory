using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts.UserManagement;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Web.UserManagement;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class UserManagementContractsMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<UserManagementContractsMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        fixture.CustomizeId<UserId>();
        fixture.CustomizeEmail();
        return new()
        {
            { fixture.Create<UserId>(), typeof(Ulid) },
            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<RegisterRequest>(), typeof(RegisterCommand) },
            { fixture.Create<RegisterRequest>(), typeof(UserIdQuery) },
            { fixture.Create<UserIdResult>(), typeof(RegisterResponse) },
        };
    }
}
