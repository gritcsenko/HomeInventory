using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly RegisterCommand _command;
    private readonly UserId _userId;

    public RegisterCommandHandlerTests()
    {
        Fixture.CustomizeUlidId<UserId>();
        Fixture.CustomizeEmail();

        _userId = Fixture.Create<UserId>();
        Fixture.CustomizeFromFactory<Ulid, ISupplier<Ulid>>(_ => new ValueSupplier<Ulid>(_userId.Value));

        _command = Fixture.Create<RegisterCommand>();
    }

    private RegisterCommandHandler CreateSut() => new(_userRepository, DateTime, _hasher);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        _userRepository.IsUserHasEmailAsync(_command.Email, Cancellation.Token).Returns(false);
#pragma warning disable CA2012 // Use ValueTasks correctly
        _userRepository.AddAsync(Arg.Any<User>(), Cancellation.Token).Returns(ValueTask.CompletedTask);
#pragma warning restore CA2012 // Use ValueTasks correctly

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(0);
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given
        _userRepository.IsUserHasEmailAsync(_command.Email, Cancellation.Token).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
            .Which.Should().BeOfType<DuplicateEmailError>();
#pragma warning disable CA2012 // Use ValueTasks correctly
        _ = _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), Cancellation.Token);
#pragma warning restore CA2012 // Use ValueTasks correctly
    }
}