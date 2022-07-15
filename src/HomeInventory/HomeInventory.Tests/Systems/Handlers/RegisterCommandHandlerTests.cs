using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Systems.Controllers;
using NSubstitute;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Handlers;
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly IUserIdFactory _userIdFactory = Substitute.For<IUserIdFactory>();
    private readonly RegisterCommand _command;

    public RegisterCommandHandlerTests()
    {
        _command = Fixture.Create<RegisterCommand>();
        Fixture.Customize(new UserIdCustomization());
    }

    private RegisterCommandHandler CreateSut() => new(_userRepository, _userIdFactory);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        _userRepository.HasEmailAsync(_command.Email, CancellationToken).Returns(false);
        _userIdFactory.CreateNew().ReturnsForAnyArgs(Fixture.Create<UserId>());
        _userRepository.AddAsync(Arg.Is<User>(r =>
            r.FirstName == _command.FirstName
            && r.LastName == _command.LastName
            && r.Email == _command.Email
            && r.Password == _command.Password), CancellationToken)
            .Returns(Task.FromResult<OneOf<Success>>(new Success()));

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_OnUserIdCreation_ReturnsError()
    {
        // Given
        var error = Errors.User.UserIdCreation;
        _userRepository.HasEmailAsync(_command.Email, CancellationToken).Returns(false);
        _userIdFactory.CreateNew().ReturnsForAnyArgs(ErrorOr.Error.Failure());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Equals(error));
        _ = _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>());
    }

    [Fact]
    public async Task Handle_OnFailure_ReturnsError()
    {
        // Given
        var error = Errors.User.DuplicateEmail;

        _userRepository.HasEmailAsync(_command.Email, CancellationToken).Returns(true);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Equals(error));
        _ = _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>());
    }
}