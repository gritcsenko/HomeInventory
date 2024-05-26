using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.ValueObjects;
using Visus.Cuid;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ScopeAccessor _scopeAccessor = new();

    public RegisterCommandHandlerTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        Fixture.CustomizeFromFactory<RegisterCommand, Email, ISupplier<Cuid>>((e, s) => new RegisterCommand(e, s.Invoke().ToString()));
    }

    private RegisterCommandHandler CreateSut() => new(_scopeAccessor, DateTime, _hasher, IdSuppliers.Cuid);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        using var token = _scopeAccessor.GetScope<IUserRepository>().Set(_userRepository);
        var command = Fixture.Create<RegisterCommand>();
        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(false);
#pragma warning disable CA2012 // Use ValueTasks correctly
        _userRepository.AddAsync(Arg.Any<User>(), Cancellation.Token).Returns(ValueTask.CompletedTask);
#pragma warning restore CA2012 // Use ValueTasks correctly

        var sut = CreateSut();
        // When
        var result = await sut.Handle(command, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(0);
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given
        using var token = _scopeAccessor.GetScope<IUserRepository>().Set(_userRepository);
        var command = Fixture.Create<RegisterCommand>();
        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(command, Cancellation.Token);
        // Then
        using var scope = new AssertionScope();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
            .Which.Should().BeOfType<DuplicateEmailError>();
        await _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), Cancellation.Token);
    }
}