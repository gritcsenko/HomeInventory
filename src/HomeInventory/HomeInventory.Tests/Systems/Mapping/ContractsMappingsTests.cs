using AutoFixture.Kernel;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Contracts;
using HomeInventory.Domain.UserManagement.ValueObjects;
using HomeInventory.Web.Mapping;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ContractsMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(Type source, Type destination)
    {
        var fixture = new Fixture();
        fixture.CustomizeId<UserId>();
        fixture.CustomizeEmail();
        var instance = fixture.Create(source, new SpecimenContext(fixture));

        var sut = CreateSut<ContractsMappings>();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<Type, Type> Data() =>
        new()
        {
            { typeof(LoginRequest), typeof(AuthenticateQuery) },
            { typeof(AuthenticateResult), typeof(LoginResponse) },
        };
}
