using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UserIdQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly ScopeAccessor _scopeAccessor = new();
    private readonly ServiceProvider _services;

    public UserIdQueryHandlerTests()
    {
        Fixture.CustomizeEmail();
        AddDisposable(_scopeAccessor.GetScope<IUserRepository>().Set(_userRepository));
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddMessageHub(AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        Fixture.CustomizeId<UserId>();
        var _user = Fixture.Create<User>();
        var query = Hub.CreateMessage((id, on) => new UserIdQueryMessage(id, on, _user.Email));

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.HandleAsync(Hub, query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(0);
        var subject = result.Value
            .Should().BeOfType<UserIdResult>()
            .Subject;
        subject.UserId.Should().Be(_user.Id);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<UserIdQueryMessage>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Optional.None<User>());

        var sut = CreateSut();
        // When
        var result = await sut.HandleAsync(Hub, query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
           .Which.Should().BeOfType<NotFoundError>()
           .Which.Message.Should().Contain(query.Email.ToString());
    }

    private IMessageHub Hub => _services.GetRequiredService<IMessageHub>();

    private UserIdQueryMessageHandler CreateSut() => new(_scopeAccessor);
}
