﻿using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Application.UserManagement.Queries;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UserIdQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));

    public UserIdQueryHandlerTests()
    {
        Fixture.CustomizeEmail();
        AddDisposable(_scopeAccessor.GetScope<IUserRepository>().Set(_userRepository));
    }

    private UserIdQueryHandler CreateSut() => new(_scopeAccessor);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        Fixture.CustomizeId<UserId>();
        var _user = Fixture.Create<User>();
        var query = new UserIdQuery(_user.Email);

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();

        // When
        var result = await sut.Handle(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeSuccess(x => x.UserId.Should().Be(_user.Id));
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<UserIdQuery>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Option<User>.None);

        var sut = CreateSut();

        // When
        var result = await sut.Handle(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
           .Which.Head.Should().BeOfType<NotFoundError>()
           .Which.Message.Should().Contain(query.Email.ToString());
    }
}
