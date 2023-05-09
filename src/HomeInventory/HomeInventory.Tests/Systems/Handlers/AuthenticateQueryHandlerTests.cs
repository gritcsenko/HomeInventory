using AutoMapper;
using HomeInventory.Application.Cqrs.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.ValueObjects;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class AuthenticateQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly User _user;

    public AuthenticateQueryHandlerTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _user = Fixture.Create<User>();
    }

    private AuthenticateQueryHandler CreateSut() => new(_tokenGenerator, _userRepository, _mapper);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, _user.Password);
        var specification = MapToSpecification(query);
        var token = Fixture.Create<string>();

        _userRepository.FindFirstOrNotFoundAsync(specification, Cancellation.Token).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, Cancellation.Token).Returns(token);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.IsT1.Should().BeFalse();
        result.AsT0.Should().NotBeNull();
        result.AsT0.Id.Should().Be(_user.Id);
        result.AsT0.Token.Should().Be(token);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<AuthenticateQuery>();
        var specification = MapToSpecification(query);
        _userRepository.FindFirstOrNotFoundAsync(specification, Cancellation.Token).Returns(new NotFound());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeAssignableTo<InvalidCredentialsError>();
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        var specification = MapToSpecification(query);
        _userRepository.FindFirstOrNotFoundAsync(specification, Cancellation.Token).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeAssignableTo<InvalidCredentialsError>();
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    private UserHasEmailSpecification MapToSpecification(AuthenticateQuery query)
    {
        var specification = new UserHasEmailSpecification(query.Email);
        _mapper.Map<FilterSpecification<User>>(query).Returns(specification);
        return specification;
    }
}
