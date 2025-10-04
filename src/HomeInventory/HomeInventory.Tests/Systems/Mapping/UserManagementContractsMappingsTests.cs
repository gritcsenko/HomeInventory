using AutoFixture.Kernel;
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
    public void ShouldMap(Type source, Type destination)
    {
        var fixture = new Fixture();
        fixture.CustomizeId<UserId>();
        fixture.CustomizeEmail();
        var instance = fixture.Create(source, new SpecimenContext(fixture));
        var sut = CreateSut<UserManagementContractsMappings>();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<Type, Type> Data() =>
        new()
        {
            { typeof(UserId), typeof(Ulid) },
            { typeof(Email), typeof(string) },
            { typeof(RegisterRequest), typeof(RegisterCommand) },
            { typeof(RegisterRequest), typeof(UserIdQuery) },
            { typeof(UserIdResult), typeof(RegisterResponse) },
        };
}
