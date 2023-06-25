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
        Fixture.CustomizeUlidId<UserId>();
        Fixture.CustomizeEmail();

        _user = Fixture.Create<User>();
        _userModel = Fixture.Build<UserModel>()
            .With(x => x.Id, _user.Id)
            .With(x => x.Email, _user.Email.Value)
            .With(x => x.Password, _user.Password)
            .Create();
    }

    [Fact]
    public async Task AddAsync_Should_CreateUser_AccordingToSpec()
    {
        var entitiesSaved = 0;
        var sut = CreateSut();
        Context.SavedChanges += (_, e) => entitiesSaved += e.EntitiesSavedCount;

        await sut.AddAsync(_user, Cancellation.Token);
        await Context.SaveChangesAsync();

        entitiesSaved.Should().Be(1);
    }

    [Fact]
    public async Task HasAsync_Should_ReturnTrue_WhenUserAdded()
    {
        Context.Set<UserModel>().Add(_userModel);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.IsUserHasEmailAsync(_user.Email, Cancellation.Token);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task FindFirstOrNotFoundAsync_Should_ReturnCorrectUser_WhenUserAdded()
    {
        Context.Set<UserModel>().Add(_userModel);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.FindFirstByEmailUserOptionalAsync(_user.Email, Cancellation.Token);

        result.Should().HaveValue(_user);
    }

    [Fact]
    public async Task HasPermissionAsync_Should_ReturnTreu_WhenUserAdded()
    {
        var permission = Fixture.Create<string>();
        Context.Set<UserModel>().Add(_userModel);
        await Context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.HasPermissionAsync(_user.Id, permission, Cancellation.Token);

        result.Should().BeTrue();
    }

    private UserRepository CreateSut() => new(Context, Mapper, SpecificationEvaluator.Default);
}
