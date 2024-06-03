using FluentAssertions.Common;
using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
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
    private readonly IScopeAccessor _scopeAccessor = new ScopeAccessor();
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
        services.AddMessageHub(AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = Hub.CreateMessage((id, on) => new AuthenticateRequestMessage(id, on, _user.Email, _user.Password));
        var token = Fixture.Create<string>();

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, Cancellation.Token).Returns(token);
        _hasher.VarifyHashAsync(query.Password, _user.Password, Cancellation.Token).Returns(true);
        using var _ = _scopeAccessor.Set(_userRepository);

        var sut = CreateSut();
        // When
        var result = await sut.HandleAsync(Hub, query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(0);
        var subject = result.Value
            .Should().BeOfType<AuthenticateResult>()
            .Subject;
        subject.Id.Should().Be(_user.Id);
        subject.Token.Should().Be(token);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<AuthenticateRequestMessage>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Optional.None<User>());
        using var _ = _scopeAccessor.Set(_userRepository);

        var sut = CreateSut();
        // When
        var result = await sut.HandleAsync(Hub, query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
           .Which.Should().BeOfType<InvalidCredentialsError>();
        await _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var query = Hub.CreateMessage((id, on) => new AuthenticateRequestMessage(id, on, _user.Email, Fixture.Create<string>()));
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);
        using var _ = _scopeAccessor.Set(_userRepository);

        var sut = CreateSut();
        // When
        var result = await sut.HandleAsync(Hub, query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
           .Which.Should().BeOfType<InvalidCredentialsError>();
        await _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
    }


    private IMessageHub Hub => _services.GetRequiredService<IMessageHub>();

    private AuthenticateRequestHandler CreateSut() => new(_tokenGenerator, _scopeAccessor, _hasher);
}
