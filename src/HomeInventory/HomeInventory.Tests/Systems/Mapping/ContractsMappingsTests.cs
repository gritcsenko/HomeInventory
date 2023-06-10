﻿using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Cqrs.Queries.Areas;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Contracts;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Web.Mapping;

namespace HomeInventory.Tests.Systems.Mapping;

[UnitTest]
public class ContractsMappingsTests : BaseMappingsTests
{
    [Theory]
    [MemberData(nameof(Data))]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Test")]
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
        fixture.CustomizeGuidId<UserId>();
        fixture.CustomizeGuidId<StorageAreaId>();
        fixture.CustomizeString(name => new StorageAreaName(name));
        fixture.CustomizeEmail();
        return new()
        {
            { fixture.Create<UserId>(), typeof(Guid) },
            { fixture.Create<Email>(), typeof(string) },
            { fixture.Create<RegisterRequest>(), typeof(RegisterCommand) },
            { fixture.Create<RegisterRequest>(), typeof(UserIdQuery) },
            { fixture.Create<UserIdResult>(), typeof(RegisterResponse) },
            { fixture.Create<LoginRequest>(), typeof(AuthenticateQuery) },
            { fixture.Create<AuthenticateResult>(), typeof(LoginResponse) },
            { fixture.Create<AreasResult>(), typeof(AreaResponse[]) },
        };
    }
}
