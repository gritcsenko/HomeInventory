using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;

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
        fixture.CustomizeUlid();
        fixture.CustomizeUlidId<UserId>();
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
