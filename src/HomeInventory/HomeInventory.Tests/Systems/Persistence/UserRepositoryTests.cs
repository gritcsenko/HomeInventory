using AutoMapper;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserRepositoryTests : BaseRepositoryTest
{
    private readonly IUserIdFactory _userIdFactory;
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly User _user;
    private readonly UserModel _userModel;

    public UserRepositoryTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();
        _userIdFactory = Substitute.For<IUserIdFactory>();
        _context = Substitute.For<IDatabaseContext>();
        _mapper = Substitute.For<IMapper>();

        Fixture.CustomizeEmail();
        _user = Fixture.Create<User>();
        _userModel = Fixture.Build<UserModel>()
            .With(x => x.Id, _user.Id.Id)
            .With(x => x.Email, _user.Email.Value)
            .With(x => x.Password, _user.Password)
        .Create();

        Mapper.Map<User, UserModel>(_user).Returns(_userModel);
        Mapper.Map<UserModel, User>(_userModel).Returns(_user);
    }

    [Fact]
    public async Task CreateAsync_Should_CreateUser_AccordingToSpec()
    {
        var id = Fixture.Create<UserId>();
        _userIdFactory.CreateNew().Returns(id);
        var spec = Fixture.Create<CreateUserSpecification>();
        var sut = CreateSut();

        var result = await sut.CreateAsync(spec, CancellationToken);

        var user = result.AsT0;
        user.Should().NotBeNull();
        user.Id.Should().Be(id);
        user.FirstName.Should().Be(spec.FirstName);
        user.LastName.Should().Be(spec.LastName);
        user.Email.Should().Be(spec.Email);
        user.Password.Should().Be(spec.Password);
    }

    [Fact]
    public async Task HasAsync_Should_RetrunTrue_WhenUserAdded()
    {
        var id = Fixture.Create<UserId>();
        _userIdFactory.CreateNew().Returns(id);
        var spec = Fixture.Create<CreateUserSpecification>();
        var sut = CreateSut();
        await sut.CreateAsync(spec, CancellationToken);

        var result = await sut.HasAsync(UserSpecifications.HasId(id), CancellationToken);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task FindFirstOrNotFoundAsync_Should_RetrunCorrectUser_WhenUserAdded()
    {
        var id = Fixture.Create<UserId>();
        _userIdFactory.CreateNew().Returns(id);
        var spec = Fixture.Create<CreateUserSpecification>();
        var sut = CreateSut();
        var expected = await sut.CreateAsync(spec, CancellationToken);

        var result = await sut.FindFirstOrNotFoundAsync(UserSpecifications.HasId(id), CancellationToken);

        var actual = result.AsT0;
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected.AsT0);
    }

    private UserRepository CreateSut() => new(_userIdFactory, _context, _mapper);
}
