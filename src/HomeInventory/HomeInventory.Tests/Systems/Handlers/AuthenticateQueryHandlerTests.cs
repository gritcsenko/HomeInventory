using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Systems.Controllers;
using MapsterMapper;
using NSubstitute;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

public class AuthenticateQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly User _user;

    public AuthenticateQueryHandlerTests()
    {
        Fixture.Customize(new UserIdCustomization());
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

        _userRepository.FindFirstOrNotFoundAsync(specification, CancellationToken).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, CancellationToken).Returns(token);

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
        var specification = MapToSpecification(query);
        _userRepository.FindFirstOrNotFoundAsync(specification, CancellationToken).Returns(new NotFound());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Equals(error));
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_OnBadPassword_ReturnsError()
    {
        // Given
        var error = Errors.Authentication.InvalidCredentials;
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        var specification = MapToSpecification(query);
        _userRepository.FindFirstOrNotFoundAsync(specification, CancellationToken).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Equals(error));
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>());
    }

    private UserHasEmailSpecification MapToSpecification(AuthenticateQuery query)
    {
        var specification = new UserHasEmailSpecification(query.Email);
        _mapper.Map<UserHasEmailSpecification>(query).Returns(specification);
        return specification;
    }
}
