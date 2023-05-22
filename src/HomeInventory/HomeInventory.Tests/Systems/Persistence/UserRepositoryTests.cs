using AutoMapper;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class UserRepositoryTests : BaseRepositoryTest
{
    private readonly IDatabaseContext _context = Substitute.For<IDatabaseContext>();
    private readonly IMapper _mapper = Substitute.For<IMapper>();
    private readonly User _user;
    private readonly UserModel _userModel;

    public UserRepositoryTests()
    {
        Fixture.CustomizeGuidId(guid => new UserId(guid));
        Fixture.CustomizeEmail();

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
        var userId = Fixture.Create<UserId>();
        Fixture.CustomizeFromFactory<Guid, ISupplier<Guid>>(_ => new ValueSupplier<Guid>(userId.Id));
        var spec = Fixture.Create<CreateUserSpecification>();
        var sut = CreateSut();

        var result = await sut.CreateAsync(spec, Cancellation.Token);

        var user = result.AsT0;
        user.Should().NotBeNull();
        user.Id.Id.Should().Be(userId.Id);
        user.Email.Should().Be(spec.Email);
        user.Password.Should().Be(spec.Password);
    }

    [Fact]
    public async Task HasAsync_Should_RetrunTrue_WhenUserAdded()
    {
        var userId = Fixture.Create<UserId>();
        Fixture.CustomizeFromFactory<Guid, ISupplier<Guid>>(_ => new ValueSupplier<Guid>(userId.Id));
        var spec = Fixture.Create<CreateUserSpecification>();
        var sut = CreateSut();
        await sut.CreateAsync(spec, Cancellation.Token);

        var result = await sut.HasAsync(UserSpecifications.HasId(userId), Cancellation.Token);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task FindFirstOrNotFoundAsync_Should_RetrunCorrectUser_WhenUserAdded()
    {
        var userId = Fixture.Create<UserId>();
        Fixture.CustomizeFromFactory<Guid, ISupplier<Guid>>(_ => new ValueSupplier<Guid>(userId.Id));
        var spec = Fixture.Create<CreateUserSpecification>();
        var sut = CreateSut();
        var expected = await sut.CreateAsync(spec, Cancellation.Token);

        var result = await sut.FindFirstOrNotFoundAsync(UserSpecifications.HasId(userId), Cancellation.Token);

        var actual = result.AsT0;
        actual.Should().NotBeNull();
        actual.Should().BeEquivalentTo(expected.AsT0);
    }

    private UserRepository CreateSut() => new(_context, _mapper);
}
