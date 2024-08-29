using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UserIdQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IRequestContext<UserIdQueryMessage> _context = Substitute.For<IRequestContext<UserIdQueryMessage>>();
    private readonly ServiceProvider _services;

    public UserIdQueryHandlerTests()
    {
        Fixture.CustomizeEmail();
        var services = new ServiceCollection();
        services.AddDomain();
        services.AddMessageHub(HomeInventory.Application.UserManagement.AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();

        AddDisposable(_services.GetRequiredService<IScopeAccessor>().Set(_userRepository));

        _context.Hub.Returns(call => _services.GetRequiredService<IMessageHub>());
        _context.RequestAborted.Returns(call => Cancellation.Token);
    }

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        Fixture.CustomizeId<UserId>();
        var _user = Fixture.Create<User>();
        var query = _context.Hub.Context.CreateMessage((id, on) => new UserIdQueryMessage(id, on, _user.Email));
        _context.Request.Returns(query);

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeSuccess(x => x.UserId.Should().Be(_user.Id));
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<UserIdQueryMessage>();
        _context.Request.Returns(query);
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(OptionNone.Default);

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
           .Which.Head.Should().BeOfType<NotFoundError>()
           .Which.Message.Should().Contain(query.Email.ToString());
    }

    private IRequestHandler<UserIdQueryMessage, IQueryResult<UserIdResult>> CreateSut() => _services.GetRequiredService<IRequestHandler<UserIdQueryMessage, IQueryResult<UserIdResult>>>();
}
