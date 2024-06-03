using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.UserManagement;
using Visus.Cuid;

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
            { fixture.Create<UserId>(), typeof(Cuid) },
            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<RegisterRequest>(), typeof(RegisterUserRequestMessage) },
            { fixture.Create<RegisterRequest>(), typeof(UserIdQueryMessage) },
            { fixture.Create<UserIdResult>(), typeof(RegisterResponse) },
        };
    }
}
