using FluentAssertions.Execution;
using HomeInventory.Application.Cqrs.Commands.Register;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.Primitives.Messages;
using HomeInventory.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IRequestContext<RegisterUserRequestMessage> _context = Substitute.For<IRequestContext<RegisterUserRequestMessage>>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));
    private readonly ServiceProvider _services;
    private readonly TimeProvider _timeProvider = new FixedTimeProvider(TimeProvider.System);

    public RegisterCommandHandlerTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        Fixture.CustomizeFromFactory<RegisterUserRequestMessage, Email, IIdSupplier<Ulid>>((e, s) => new RegisterUserRequestMessage(IdSuppliers.Ulid.Supply(), DateTime.GetUtcNow(), e, s.Supply().ToString()));
        var services = new ServiceCollection();
        services.AddSingleton(typeof(ILogger<>), typeof(TestingLogger<>.Stub));
        services.AddDomain();
        services.AddMessageHub(
            HomeInventory.Application.AssemblyReference.Assembly,
            HomeInventory.Application.UserManagement.AssemblyReference.Assembly);
        _services = services.BuildServiceProvider();

        _context.Hub.Returns(call => _services.GetRequiredService<IMessageHub>());
        _context.RequestAborted.Returns(call => Cancellation.Token);
    }

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        using var token = _scopeAccessor.GetScope<IUserRepository>().Set(_userRepository);
        var command = Fixture.Create<RegisterUserRequestMessage>();
        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(false);
#pragma warning disable CA2012 // Use ValueTasks correctly
        _userRepository.AddAsync(Arg.Any<User>(), Cancellation.Token).Returns(Task.CompletedTask);
#pragma warning restore CA2012 // Use ValueTasks correctly

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeNone();
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given
        using var token = _scopeAccessor.GetScope<IUserRepository>().Set(_userRepository);
        var command = Fixture.Create<RegisterUserRequestMessage>();
        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(true);

        var sut = CreateSut();

        // When
        var result = await sut.HandleAsync(_context);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeSome(error => error.Should().BeOfType<DuplicateEmailError>());
        await _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), Cancellation.Token);
    }

    private RegisterUserRequestHandler CreateSut() => new(_scopeAccessor, _hasher, IdSuppliers.Ulid, _timeProvider);
}