using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Application.UserManagement.Services;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class AuthenticationServiceTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly User _user;

    public AuthenticationServiceTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        _user = Fixture.Create<User>();
    }

    private AuthenticationService CreateSut() => new(_tokenGenerator, _userRepository, _hasher);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, _user.Password);
        var token = Fixture.Create<string>();

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, Cancellation.Token).Returns(token);
        _hasher.VarifyHashAsync(query.Password, _user.Password, Cancellation.Token).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.AuthenticateAsync(query, Cancellation.Token);
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
        var query = Fixture.Create<AuthenticateQuery>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Option<User>.None);

        var sut = CreateSut();
        // When
        var result = await sut.AuthenticateAsync(query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
            .Which.Head.Should().BeOfType<InvalidCredentialsError>();
#pragma warning disable CA2012 // Use ValueTasks correctly
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
#pragma warning restore CA2012 // Use ValueTasks correctly
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.AuthenticateAsync(query, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
            .Which.Head.Should().BeOfType<InvalidCredentialsError>();
#pragma warning disable CA2012 // Use ValueTasks correctly
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
#pragma warning restore CA2012 // Use ValueTasks correctly
    }
}
