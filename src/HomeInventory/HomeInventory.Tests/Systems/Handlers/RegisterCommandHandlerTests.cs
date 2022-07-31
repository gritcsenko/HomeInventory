using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using MapsterMapper;
using NSubstitute;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

[Trait("Category", "Unit")]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly RegisterCommand _command;
    private readonly UserHasEmailSpecification _userHasEmailSpecification;
    private readonly CreateUserSpecification _createUserSpecification;

    public RegisterCommandHandlerTests()
    {
        Fixture.Customize(new UserIdCustomization());

        _command = Fixture.Create<RegisterCommand>();

        _userHasEmailSpecification = new UserHasEmailSpecification(_command.Email);
        _createUserSpecification = new CreateUserSpecification(_command.FirstName, _command.LastName, _command.Email, _command.Password);
        _mapper.Map<FilterSpecification<User>>(_command).Returns(_userHasEmailSpecification);
        _mapper.Map<CreateUserSpecification>(_command).Returns(_createUserSpecification);
    }

    private RegisterCommandHandler CreateSut() => new(_userRepository, _mapper);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var user = Fixture.Create<User>();
        _userRepository.HasAsync(_userHasEmailSpecification, CancellationToken).Returns(false);
        _userRepository.CreateAsync(_createUserSpecification, CancellationToken).Returns(user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_OnUserCreation_ReturnsError()
    {
        // Given
        _userRepository.HasAsync(_userHasEmailSpecification, CancellationToken).Returns(false);
        _userRepository.CreateAsync(_createUserSpecification, CancellationToken).Returns(new None());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1).And.OnlyContain(e => e.Equals(Errors.User.UserCreation));
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given

        _userRepository.HasAsync(_userHasEmailSpecification, CancellationToken).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().HaveCount(1).And.OnlyContain(e => e.Equals(Errors.User.DuplicateEmail));
        _ = _userRepository.DidNotReceiveWithAnyArgs().CreateAsync(_createUserSpecification);
    }
}