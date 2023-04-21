using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[Trait("Category", "Unit")]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IIdFactory<UserId> _idFactory = Substitute.For<IIdFactory<UserId>>();
    private readonly RegisterCommand _command;
    private readonly UserId _userId;

    public RegisterCommandHandlerTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

        _command = Fixture.Create<RegisterCommand>();
        _userId = Fixture.Create<UserId>();
        _idFactory.CreateNew().Returns(_userId);
    }

    private RegisterCommandHandler CreateSut() => new(_userRepository, _idFactory);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        _userRepository.IsUserHasEmailAsync(_command.Email, CancellationToken).Returns(false);
        _userRepository.AddAsync(Arg.Any<User>(), CancellationToken).Returns(ci => Task.FromResult(ci.Arg<User>()));

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.Index.Should().Be(0);
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given
        _userRepository.IsUserHasEmailAsync(_command.Email, CancellationToken).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
            .Which.Should().BeOfType<DuplicateEmailError>();
        _ = _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), CancellationToken);
    }
}