using Ardalis.Specification.EntityFrameworkCore;
using AutoFixture;
using AutoMapper;
using FluentAssertions;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Tests.Helpers;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserRepositoryTests : BaseTest
{
    private readonly DatabaseContext _context = HomeInventory.Domain.TypeExtensions.CreateInstance<DatabaseContext>(GetDatabaseOptions())!;

    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly User _user;
    private readonly UserModel _userModel;

    public UserRepositoryTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

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
        var sut = CreateSut();

        await sut.AddAsync(_user, CancellationToken);

        _context.Set<UserModel>().Local.Should().HaveCount(1);
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

    private UserRepository CreateSut() => new(_context, _mapper, SpecificationEvaluator.Default);
}
