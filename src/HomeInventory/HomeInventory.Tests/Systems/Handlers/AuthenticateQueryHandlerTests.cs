using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using NSubstitute;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

[Trait("Category", "Unit")]
public class AuthenticateQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly User _user;

    public AuthenticateQueryHandlerTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.Customize(new EmailCustomization());
        _user = Fixture.Create<User>();
    }

    private AuthenticateQueryHandler CreateSut() => new(_tokenGenerator, _userRepository);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, _user.Password);
        var token = Fixture.Create<string>();

        _userRepository.FindFirstByEmailOrNotFoundUserAsync(query.Email, CancellationToken).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, CancellationToken).Returns(token);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_user.Id);
        result.Value.Token.Should().Be(token);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<AuthenticateQuery>();
        _userRepository.FindFirstByEmailOrNotFoundUserAsync(query.Email, CancellationToken).Returns(new NotFound());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is InvalidCredentialsError);
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        _userRepository.FindFirstByEmailOrNotFoundUserAsync(query.Email, CancellationToken).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e is InvalidCredentialsError);
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }
}
