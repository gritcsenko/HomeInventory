using HomeInventory.Application.Cqrs.Queries.UserId;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Systems.Handlers;

[UnitTest]
public class UserIdQueryHandlerTests : BaseTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    public UserIdQueryHandlerTests()
    {
        Fixture.CustomizeEmail();
    }

    private UserIdQueryHandler CreateSut() => new(_userRepository);

    [Fact]
    public async Task Handle_OnSuccess_ReturnsResult()
    {
        // Given
        Fixture.CustomizeUlidId<UserId>();
        var _user = Fixture.Create<User>();
        var query = new UserIdQuery(_user.Email);

        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(_user);

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.Index.Should().Be(0);
        var subject = result.Value
            .Should().BeOfType<UserIdResult>()
            .Subject;
        subject.UserId.Should().Be(_user.Id);
    }

    [Fact]
    public async Task Handle_OnNotFound_ReturnsError()
    {
        // Given
        var query = Fixture.Create<UserIdQuery>();
        _userRepository.FindFirstByEmailUserOptionalAsync(query.Email, Cancellation.Token).Returns(Optional.None<User>());

        var sut = CreateSut();
        // When
        var result = await sut.Handle(query, Cancellation.Token);
        // Then
        result.Should().NotBeNull();
        result.Index.Should().Be(1);
        result.Value.Should().BeAssignableTo<IError>()
           .Which.Should().BeOfType<NotFoundError>()
           .Which.Message.Should().Contain(query.Email.ToString());
    }
}
