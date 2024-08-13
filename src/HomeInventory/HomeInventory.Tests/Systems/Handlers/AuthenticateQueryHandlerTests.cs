using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class AuthenticateQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IRequestContext<AuthenticateRequestMessage> _context = Substitute.For<IRequestContext<AuthenticateRequestMessage>>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));
    private readonly User _user;
    private readonly ServiceProvider _services;

    public AuthenticateQueryHandlerTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        _user = Fixture.Create<User>();
        var services = new ServiceCollection();
        services.AddSingleton(_scopeAccessor);
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddDomain();
        services.AddMessageHub(
            HomeInventory.Application.AssemblyReference.Assembly,
            HomeInventory.Application.UserManagement.AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();

        _context.Hub.Returns(call => _services.GetRequiredService<IMessageHub>());
        _context.RequestAborted.Returns(call => Cancellation.Token);
    }

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = _context.Hub.CreateMessage((id, on) => new AuthenticateRequestMessage(id, on, _user.Email, _user.Password));
        _context.Request.Returns(query);
        var token = Fixture.Create<string>();

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, Cancellation.Token).Returns(token);
        _hasher.VarifyHashAsync(query.Password, _user.Password, Cancellation.Token).Returns(true);
        using var _ = _scopeAccessor.Set(_userRepository);

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        var subject = result.Should().BeSuccess().Subject;
        subject.Id.Should().Be(_user.Id);
        subject.Token.Should().Be(token);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<AuthenticateRequestMessage>();
        _context.Request.Returns(query);
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(OptionNone.Default);
        using var _ = _scopeAccessor.Set(_userRepository);

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
            .Which.Head.Should().BeOfType<InvalidCredentialsError>();
        await _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var query = _context.Hub.CreateMessage((id, on) => new AuthenticateRequestMessage(id, on, _user.Email, Fixture.Create<string>()));
        _context.Request.Returns(query);
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);
        using var _ = _scopeAccessor.Set(_userRepository);

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
            .Which.Head.Should().BeOfType<InvalidCredentialsError>();
        await _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
    }

    private AuthenticateRequestHandler CreateSut() => new(_tokenGenerator, _scopeAccessor, _hasher);
}
