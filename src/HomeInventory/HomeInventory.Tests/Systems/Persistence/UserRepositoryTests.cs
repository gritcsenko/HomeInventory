using Ardalis.Specification.EntityFrameworkCore;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class UserRepositoryTests : BaseRepositoryTest
{
    private readonly User _user;
    private readonly UserModel _userModel;

    public UserRepositoryTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

        _user = Fixture.Create<User>();
        _userModel = Fixture.Build<UserModel>()
            .With(x => x.Id, _user.Id)
            .With(x => x.Email, _user.Email.Value)
            .With(x => x.Password, _user.Password)
            .Create();

        Mapper.Map<User, UserModel>(_user).Returns(_userModel);
        Mapper.Map<UserModel, User>(_userModel).Returns(_user);
    }

    [Fact]
    public async Task AddAsync_Should_CreateUser_AccordingToSpec()
    {
        var entitiesSaved = 0;
        var sut = CreateSut();
        await using var unit = await sut.WithUnitOfWorkAsync(CancellationToken);
        Context.SavedChanges += (_, e) => entitiesSaved += e.EntitiesSavedCount;

        await sut.AddAsync(_user, CancellationToken);
        await unit.SaveChangesAsync(CancellationToken);

        entitiesSaved.Should().Be(1);
    }

    [Fact]
    public async Task HasAsync_Should_ReturnTrue_WhenUserAdded()
    {
        Context.Set<UserModel>().Add(_userModel);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.IsUserHasEmailAsync(_user.Email, CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task FindFirstOrNotFoundAsync_Should_ReturnCorrectUser_WhenUserAdded()
    {
        Mapper.ProjectTo<User>(Arg.Any<IQueryable>(), CancellationToken).Returns(ci =>
        {
            var query = ci.Arg<IQueryable>();
            var userModels = query.Cast<UserModel>();
            return userModels.Select(x => _user);
        });
        Context.Set<UserModel>().Add(_userModel);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.FindFirstByEmailOrNotFoundUserAsync(_user.Email, CancellationToken);

        var actual = result.AsT0;
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(_user);
    }

    private UserRepository CreateSut() => new(Factory, Mapper, SpecificationEvaluator.Default, DateTimeService);
}
