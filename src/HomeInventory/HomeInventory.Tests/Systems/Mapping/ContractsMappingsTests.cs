using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Mapping;

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
        fixture.CustomizeId<UserId>();
        fixture.CustomizeId<StorageAreaId>();
        fixture.CustomizeFromFactory<StorageAreaName, string>(x => new(x));
        fixture.CustomizeEmail();
        return new()
        {
            { fixture.Create<LoginRequest>(), typeof(AuthenticateQuery) },
            { fixture.Create<AuthenticateResult>(), typeof(LoginResponse) },
            { fixture.Create<AreasResult>(), typeof(AreaResponse[]) },
        };
    }
}
