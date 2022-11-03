using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.Entities;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserRepositoryTests : BaseTest
{
    private readonly IDatabaseContext _context = Substitute.For<IDatabaseContext>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly ISpecificationEvaluator _evaluator = Substitute.For<ISpecificationEvaluator>();
    private readonly DbSet<UserModel> _set = Substitute.For<DbSet<UserModel>, IQueryable<UserModel>>();
    private readonly User _user;
    private readonly UserModel _userModel;

    public UserRepositoryTests()
    {
        Fixture.Customize(new UserIdCustomization());
        _context.Set<UserModel>().Returns(_set);
        _user = Fixture.Create<User>();
        _userModel = Fixture.Build<UserModel>()
            .With(x => x.Id, _user.Id.Id)
            .With(x => x.Email, _user.Email)
            .With(x => x.Password, _user.Password)
            .With(x => x.FirstName, _user.FirstName)
            .With(x => x.LastName, _user.LastName)
           .Create();
        _mapper.Map<UserModel>(_user).Returns(_userModel);
        _mapper.Map<User>(_userModel).Returns(_user);

        _evaluator.FilterBy(Arg.Any<IQueryable<UserModel>>(), Arg.Any<IFilterSpecification<UserModel>>())
            .Returns(ci => ci.Arg<IQueryable<UserModel>>().Where(ci.Arg<IFilterSpecification<UserModel>>().QueryExpression));
    }

    [Fact]
    public async Task AddAsync_Should_CreateUser_AccordingToSpec()
    {
        var sut = CreateSut();

        var actual = await sut.AddAsync(_user, CancellationToken);

        Received.InOrder(() =>
        {
            _ = _set.AddAsync(_userModel, CancellationToken);
            _ = _context.SaveChangesAsync(CancellationToken);
        });
        actual.Should().NotBeNull();
        actual.Id.Should().Be(_user.Id);
        actual.FirstName.Should().Be(_user.FirstName);
        actual.LastName.Should().Be(_user.LastName);
        actual.Email.Should().Be(_user.Email);
        actual.Password.Should().Be(_user.Password);

    }

    [Fact]
    public async Task HasAsync_Should_ReturnTrue_WhenUserAdded()
    {
        var data = new[] { _userModel }.AsQueryable();
        _set.AsQueryable().Returns(data);

        var sut = CreateSut();

        var result = await sut.IsUserHasEmailAsync(_user.Email, CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task FindFirstOrNotFoundAsync_Should_ReturnCorrectUser_WhenUserAdded()
    {
        var data = new[] { _userModel }.AsQueryable();
        _set.AsQueryable().Returns(data);
        var sut = CreateSut();

        var result = await sut.FindFirstByEmailOrNotFoundUserAsync(_user.Email, CancellationToken);

        var actual = result.AsT0;
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(_user);
    }

    private UserRepository CreateSut() => new(_context, _mapper, _evaluator);
}
