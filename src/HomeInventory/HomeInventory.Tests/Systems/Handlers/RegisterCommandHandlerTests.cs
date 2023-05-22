using AutoMapper;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly RegisterCommand _command;
    private readonly UserHasEmailSpecification _userHasEmailSpecification;
    private readonly CreateUserSpecification _createUserSpecification;

    public RegisterCommandHandlerTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

        _command = Fixture.Create<RegisterCommand>();

        _userHasEmailSpecification = new UserHasEmailSpecification(_command.Email);
        _createUserSpecification = new CreateUserSpecification(_command.Email, _command.Password, new ValueSupplier<Guid>(Guid.NewGuid()));
        _mapper.Map<FilterSpecification<User>>(_command).Returns(_userHasEmailSpecification);
        _mapper.Map<CreateUserSpecification>(_command).Returns(_createUserSpecification);
    }

    private RegisterCommandHandler CreateSut() => new(_userRepository, _mapper);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        var user = Fixture.Create<User>();
        _userRepository.HasAsync(_userHasEmailSpecification, Cancellation.Token).Returns(false);
        _userRepository.CreateAsync(_createUserSpecification, Cancellation.Token).Returns(user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.IsT1.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.AsT0.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task Handle_OnUserCreation_ReturnsError()
    {
        // Given
        _userRepository.HasAsync(_userHasEmailSpecification, Cancellation.Token).Returns(false);
        _userRepository.CreateAsync(_createUserSpecification, Cancellation.Token).Returns(new None());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeAssignableTo<UserCreationError>();
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given

        _userRepository.HasAsync(_userHasEmailSpecification, Cancellation.Token).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.IsT1.Should().BeTrue();
        result.AsT1.Should().BeAssignableTo<DuplicateEmailError>();
        _ = _userRepository.DidNotReceiveWithAnyArgs().CreateAsync(_createUserSpecification);
    }
}