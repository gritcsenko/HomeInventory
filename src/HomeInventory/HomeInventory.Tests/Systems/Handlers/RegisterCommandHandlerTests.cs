using AutoFixture;
using FluentAssertions;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using NSubstitute;

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
        Fixture.Customize(new UserIdCustomization());
        Fixture.Customize(new EmailCustomization());

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
        _userRepository.AddAsync(Arg.Any<User>(), CancellationToken).Returns(ci => ci.Arg<User>());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(_command, CancellationToken);
        // Then
        result.Should().NotBeNull();
        result.IsFailed.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(_userId);
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
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().HaveCount(1).And.OnlyContain(e => e is DuplicateEmailError);
        _ = _userRepository.DidNotReceiveWithAnyArgs().AddAsync(Arg.Any<User>(), CancellationToken);
    }
}