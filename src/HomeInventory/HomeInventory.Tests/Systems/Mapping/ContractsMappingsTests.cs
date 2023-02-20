using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Mapping;
using HomeInventory.Contracts;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;
using HomeInventory.Web.Mapping;
using Microsoft.Extensions.DependencyInjection;

namespace HomeInventory.Tests.Systems.Mapping;

[Trait("Category", "Unit")]
public class ContractsMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    public void ShouldMap(object instance, Type destination)
    {
        Services.AddSingleton<IValueObjectFactory<Email, string>, EmailFactory>();
        Services.AddSingleton<ValueObjectConverter<Email, string>>();
        var sut = CreateSut<ContractsMappings>();
        var source = instance.GetType();

        var target = sut.Map(instance, source, destination);

        target.Should().BeAssignableTo(destination);
    }

    public static TheoryData<object, Type> Data()
    {
        var fixture = new Fixture();
        fixture.CustomizeGuidId(guid => new UserId(guid));
        fixture.CustomizeGuidId(guid => new StorageAreaId(guid));
        fixture.CustomizeString(name => new StorageAreaName(name));
        fixture.CustomizeEmail();
        return new()
        {
            { fixture.Create<UserId>(), typeof(Guid) },
            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<RegisterRequest>(), typeof(RegisterCommand) },
            { fixture.Create<RegistrationResult>(), typeof(RegisterResponse) },
            { fixture.Create<LoginRequest>(), typeof(AuthenticateQuery) },
            { fixture.Create<AuthenticateResult>(), typeof(LoginResponse) },
            { fixture.Create<AreasResult>(), typeof(AreaResponse[]) },
        };
    }
}
