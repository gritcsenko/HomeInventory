using AutoFixture;
using AutoMapper;
using FluentAssertions;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Tests.Customizations;
using HomeInventory.Tests.Helpers;
using NSubstitute;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class UserRepositoryTests : BaseTest
{
    private readonly IUserIdFactory _userIdFactory;
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;

    public UserRepositoryTests()
    {
        Fixture.Customize(new UserIdCustomization());
        _userIdFactory = Substitute.For<IUserIdFactory>();
        _context = Substitute.For<IDatabaseContext>();
        _mapper = Substitute.For<IMapper>();
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
