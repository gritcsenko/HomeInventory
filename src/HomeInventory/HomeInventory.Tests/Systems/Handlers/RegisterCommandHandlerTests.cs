using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using HomeInventory.Tests.Systems.Controllers;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Handlers;
public class RegisterCommandHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();
    private readonly RegisterCommand _command;

    public RegisterCommandHandlerTests()
    {
        _command = Fixture.Create<RegisterCommand>();
    }

    private RegisterCommandHandler CreateSut() => new(_userRepository);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        _userRepository.HasEmailAsync(_command.Email, CancellationToken).Returns(false);

        _userRepository.AddUserAsync(Arg.Is<User>(r =>
            r.FirstName == _command.FirstName
            && r.LastName == _command.LastName
            && r.Email == _command.Email
            && r.Password == _command.Password), CancellationToken)
            .Returns(Task.CompletedTask);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
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
        _ = _userRepository.DidNotReceiveWithAnyArgs().AddUserAsync(Arg.Any<User>());
    }
}