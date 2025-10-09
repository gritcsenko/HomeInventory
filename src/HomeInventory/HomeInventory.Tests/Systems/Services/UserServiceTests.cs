using HomeInventory.Application.UserManagement;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Errors;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Tests.Systems.Services;

[UnitTest]
public class UserServiceTests : BaseTest
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = Substitute.For<IAuthenticationTokenGenerator>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly ScopeAccessor _scopeAccessor = new(new ScopeContainer(new ScopeFactory()));
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ILogger<UserService> _logger = Substitute.For<ILogger<UserService>>();
    private readonly User _user;

    public UserServiceTests()
    {
        Fixture.CustomizeId<UserId>();
        Fixture.CustomizeEmail();
        Fixture.CustomizeUlid();
        _user = Fixture.Create<User>();

        AddDisposable(_scopeAccessor.GetScope<IUserRepository>().Set(_userRepository));
        AddDisposable(_scopeAccessor.GetScope<IUnitOfWork>().Set(_unitOfWork));
    }

    private UserService CreateSut() => new(_tokenGenerator, _hasher, _scopeAccessor, DateTime, IdSuppliers.Ulid, _logger);

    #region AuthenticateAsync Tests

    [Fact]
    public async Task AuthenticateAsync_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, _user.PasswordHash);
        var token = Fixture.Create<string>();

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);
        _tokenGenerator.GenerateTokenAsync(_user, Cancellation.Token).Returns(token);
        _hasher.VarifyHashAsync(query.Password, _user.PasswordHash, Cancellation.Token).Returns(true);

        var sut = CreateSut();

        // When
        var result = await sut.AuthenticateAsync(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        var subject = result.Should().BeSuccess().Subject;
        subject.Id.Should().Be(_user.Id);
        subject.Token.Should().Be(token);
    }

    [Fact]
    public async Task AuthenticateAsync_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<AuthenticateQuery>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Option<User>.None);

        var sut = CreateSut();

        // When
        var result = await sut.AuthenticateAsync(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
            .Which.Head.Should().BeOfType<InvalidCredentialsError>();
#pragma warning disable CA2012 // Use ValueTasks correctly
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
#pragma warning restore CA2012 // Use ValueTasks correctly
    }

    [Fact]
    public async Task AuthenticateAsync_OnBadPassword_ReturnsError()
    {
        // Given
        var query = new AuthenticateQuery(_user.Email, Fixture.Create<string>());
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();

        // When
        var result = await sut.AuthenticateAsync(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
            .Which.Head.Should().BeOfType<InvalidCredentialsError>();
#pragma warning disable CA2012 // Use ValueTasks correctly
        _ = _tokenGenerator.DidNotReceiveWithAnyArgs().GenerateTokenAsync(Arg.Any<User>(), Cancellation.Token);
#pragma warning restore CA2012 // Use ValueTasks correctly
    }

    #endregion

    #region GetUserIdAsync Tests

    [Fact]
    public async Task GetUserIdAsync_OnSuccess_ReturnsResult()
    {
        // Given
        var query = new UserIdQuery(_user.Email);

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();

        // When
        var result = await sut.GetUserIdAsync(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeSuccess(x => x.UserId.Should().Be(_user.Id));
    }

    [Fact]
    public async Task GetUserIdAsync_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<UserIdQuery>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Option<User>.None);

        var sut = CreateSut();

        // When
        var result = await sut.GetUserIdAsync(query, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeFail()
           .Which.Head.Should().BeOfType<NotFoundError>()
           .Which.Message.Should().Contain(query.Email.ToString());
    }

    #endregion

    #region RegisterAsync Tests

    [Fact]
    public async Task RegisterAsync_OnSuccess_ReturnsNone()
    {
        // Given
        Fixture.CustomizeFromFactory<RegisterCommand, Email, IIdSupplier<Ulid>>(static (e, s) => new(e, s.Supply().ToString()));
        var command = Fixture.Create<RegisterCommand>();

        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(false);
#pragma warning disable CA2012 // Use ValueTasks correctly
        _userRepository.AddAsync(Arg.Any<User>(), Cancellation.Token).Returns(Task.CompletedTask);
#pragma warning restore CA2012 // Use ValueTasks correctly
        _unitOfWork.SaveChangesAsync(Cancellation.Token).Returns(1);

        var sut = CreateSut();

        // When
        var result = await sut.RegisterAsync(command, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeNone();

        await _userRepository.Received(1).AddAsync(Arg.Any<User>(), Cancellation.Token);
        _ = _unitOfWork.Received(1).SaveChangesAsync(Cancellation.Token);
    }

    [Fact]
    public async Task RegisterAsync_OnDuplicateEmail_ReturnsError()
    {
        // Given
        Fixture.CustomizeFromFactory<RegisterCommand, Email, IIdSupplier<Ulid>>(static (e, s) => new(e, s.Supply().ToString()));
        var command = Fixture.Create<RegisterCommand>();

        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(true);

        var sut = CreateSut();

        // When
        var result = await sut.RegisterAsync(command, Cancellation.Token);

        // Then
        using var scope = new AssertionScope();
        result.Should().BeSome(static error => error.Should().BeOfType<DuplicateEmailError>());
        await _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), Cancellation.Token);
        _ = _unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(Cancellation.Token);
    }

    [Fact]
    public async Task RegisterAsync_CallsSaveChanges_OnSuccess()
    {
        // Given
        Fixture.CustomizeFromFactory<RegisterCommand, Email, IIdSupplier<Ulid>>(static (e, s) => new(e, s.Supply().ToString()));
        var command = Fixture.Create<RegisterCommand>();

        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(false);
#pragma warning disable CA2012 // Use ValueTasks correctly
        _userRepository.AddAsync(Arg.Any<User>(), Cancellation.Token).Returns(Task.CompletedTask);
#pragma warning restore CA2012 // Use ValueTasks correctly
        _unitOfWork.SaveChangesAsync(Cancellation.Token).Returns(1);

        var sut = CreateSut();

        // When
        _ = await sut.RegisterAsync(command, Cancellation.Token);

        // Then
        _ = _unitOfWork.Received(1).SaveChangesAsync(Cancellation.Token);
    }

    [Fact]
    public async Task RegisterAsync_DoesNotCallSaveChanges_OnValidationError()
    {
        // Given
        Fixture.CustomizeFromFactory<RegisterCommand, Email, IIdSupplier<Ulid>>(static (e, s) => new(e, s.Supply().ToString()));
        var command = Fixture.Create<RegisterCommand>();

        _userRepository.IsUserHasEmailAsync(command.Email, Cancellation.Token).Returns(true);

        var sut = CreateSut();

        // When
        _ = await sut.RegisterAsync(command, Cancellation.Token);

        // Then
        _ = _unitOfWork.Received(0).SaveChangesAsync(Cancellation.Token);
    }

    #endregion
}

