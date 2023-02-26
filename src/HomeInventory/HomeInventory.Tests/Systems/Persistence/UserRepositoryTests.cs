using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserRepositoryTests : BaseTest
{
    private readonly DatabaseContext _context = HomeInventory.Domain.TypeExtensions.CreateInstance<DatabaseContext>(GetDatabaseOptions())!;
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly IDateTimeService _dateTimeService = Substitute.For<IDateTimeService>();
    private readonly IDbContextFactory<DatabaseContext> _factory = Substitute.For<IDbContextFactory<DatabaseContext>>();
    private readonly User _user;
    private readonly UserModel _userModel;

    public UserRepositoryTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

        _factory.CreateDbContextAsync(CancellationToken).Returns(_context);

        _user = Fixture.Create<User>();
        _userModel = Fixture.Build<UserModel>()
            .With(x => x.Id, _user.Id.Id)
            .With(x => x.Email, _user.Email.Value)
            .With(x => x.Password, _user.Password)
            .With(x => x.FirstName, _user.FirstName)
            .With(x => x.LastName, _user.LastName)
            .Create();

        _mapper.Map<User, UserModel>(_user).Returns(_userModel);
        _mapper.Map<UserModel, User>(_userModel).Returns(_user);
    }

    private static DbContextOptions<DatabaseContext> GetDatabaseOptions()
        => new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase(databaseName: "db").Options;

    [Fact]
    public async Task AddAsync_Should_CreateUser_AccordingToSpec()
    {
        var entitiesSaved = 0;
        var sut = CreateSut();
        await using var unit = await sut.WithUnitOfWorkAsync(CancellationToken);
        _context.SavedChanges += (_, e) => entitiesSaved += e.EntitiesSavedCount;

        await sut.AddAsync(_user, CancellationToken);
        await unit.SaveChangesAsync(CancellationToken);

        entitiesSaved.Should().Be(1);
    }

    [Fact]
    public async Task HasAsync_Should_ReturnTrue_WhenUserAdded()
    {
        _context.Set<UserModel>().Add(_userModel);
        await _context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.IsUserHasEmailAsync(_user.Email, CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task FindFirstOrNotFoundAsync_Should_ReturnCorrectUser_WhenUserAdded()
    {
        _mapper.ProjectTo<User>(Arg.Any<IQueryable>(), CancellationToken).Returns(ci =>
        {
            var query = ci.Arg<IQueryable>();
            var userModels = query.Cast<UserModel>();
            return userModels.Select(x => _user);
        });
        _context.Set<UserModel>().Add(_userModel);
        await _context.SaveChangesAsync();
        var sut = CreateSut();

        var result = await sut.FindFirstByEmailOrNotFoundUserAsync(_user.Email, CancellationToken);

        var actual = result.AsT0;
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(_user);
    }

    private UserRepository CreateSut() => new(_factory, _mapper, SpecificationEvaluator.Default, _dateTimeService);
}
