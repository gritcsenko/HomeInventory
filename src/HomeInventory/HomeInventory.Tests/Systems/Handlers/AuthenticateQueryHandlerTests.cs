using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class AuthenticateQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly User _user;

    public AuthenticateQueryHandlerTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _user = Fixture.Create<User>();
    }

    private AuthenticateQueryHandler CreateSut() => new(_tokenGenerator, _userRepository);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, _user.Password);
        var token = Fixture.Create<string>();

        _userRepository.FindFirstByEmailOrNotFoundUserAsync(query.Email, Cancellation.Token).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, Cancellation.Token).Returns(token);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
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
        var query = Fixture.Create<AuthenticateQuery>();
        _userRepository.FindFirstByEmailOrNotFoundUserAsync(query.Email, Cancellation.Token).Returns(new NotFound());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
           .Which.Should().BeOfType<InvalidCredentialsError>();
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        _userRepository.FindFirstByEmailOrNotFoundUserAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
           .Which.Should().BeOfType<InvalidCredentialsError>();
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }
}
