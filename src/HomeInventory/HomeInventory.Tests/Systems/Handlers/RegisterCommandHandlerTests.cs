using HomeInventory.Application.UserManagement.Commands;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Errors;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));

    public RegisterCommandHandlerTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        Fixture.CustomizeFromFactory<RegisterCommand, Email, IIdSupplier<Ulid>>(static (e, s) => new(e, s.Supply().ToString()));
    }

    private RegisterCommandHandler CreateSut() => new(_scopeAccessor, DateTime, _hasher, IdSuppliers.Ulid);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        using var token = _scopeAccessor.GetScope<IUserRepository>().Set(_userRepository);
        var command = Fixture.Create<RegisterCommand>();
        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(false);
#pragma warning disable CA2012 // Use ValueTasks correctly
        _userRepository.AddAsync(Arg.Any<User>(), Cancellation.Token).Returns(Task.CompletedTask);
#pragma warning restore CA2012 // Use ValueTasks correctly

        var sut = CreateSut();

        // When
        var result = await sut.Handle(command, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeNone();
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
        result.Should().BeSome(static error => error.Should().BeOfType<DuplicateEmailError>());
        await _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), Cancellation.Token);
    }
}