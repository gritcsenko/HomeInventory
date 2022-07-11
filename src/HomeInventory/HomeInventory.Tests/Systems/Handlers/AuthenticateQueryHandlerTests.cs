using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Tests.Systems.Controllers;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Handlers;

public class AuthenticateQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly User _user;

    public AuthenticateQueryHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
        _user = Fixture.Create<User>();
    }

    private AuthenticateQueryHandler CreateSut()
    {
        var sut = new AuthenticateQueryHandler(_tokenGenerator, _userRepository);
        return sut;
    }

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, _user.Password);
        var token = Fixture.Create<string>();

        _userRepository.FindByEmailAsync(query.Email, CancellationToken)
            .Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, CancellationToken)
            .Returns(token);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_user.Id);
        result.Value.Token.Should().Be(token);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var error = Errors.Authentication.InvalidCredentials;
        var query = Fixture.Create<AuthenticateQuery>();
        _userRepository.FindByEmailAsync(query.Email, CancellationToken)
            .Returns(default(User?));

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == error.Code && e.Description == error.Description && e.Type == error.Type);
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var error = Errors.Authentication.InvalidCredentials;
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        _userRepository.FindByEmailAsync(query.Email, CancellationToken)
            .Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Code == error.Code && e.Description == error.Description && e.Type == error.Type);
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }
}
