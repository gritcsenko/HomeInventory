using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ContractsMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        var sut = CreateSut<ContractsMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        fixture.CustomizeUlid();
        fixture.CustomizeUlidId<UserId>();
        fixture.CustomizeUlidId<StorageAreaId>();
        fixture.CustomizeString(name => new StorageAreaName(name));
        fixture.CustomizeEmail();
        return new()
        {
            { fixture.Create<LoginRequest>(), typeof(AuthenticateQuery) },
            { fixture.Create<AuthenticateResult>(), typeof(LoginResponse) },
            { fixture.Create<AreasResult>(), typeof(AreaResponse[]) },
        };
    }
}
