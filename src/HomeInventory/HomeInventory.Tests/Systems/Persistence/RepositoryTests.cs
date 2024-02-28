using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class RepositoryTests : BaseRepositoryTest
{
    public RepositoryTests()
    {
        Fixture.CustomizeUlidId<UserId>();
    }

    [Fact]
    public async Task AddAsync_ShouldAdd()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();

        await sut.AddAsync(entity, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().ContainSingle();
    }

    [Fact]
    public async Task AddRangeAsync_ShouldAdd()
    {
        var entities = Fixture.CreateMany<User>();
        var sut = CreateSut();

        await sut.AddRangeAsync(entities, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveExisting()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        await sut.DeleteAsync(entity, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteRangeAsync_ShouldRemoveExisting()
    {
        var entities = Fixture.CreateMany<User>();
        var sut = CreateSut();
        await sut.AddRangeAsync(entities, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        await sut.DeleteRangeAsync(entities, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await Context.Set<UserModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().BeEmpty();
    }

    [Fact]
    public async Task FindFirstOptionalAsync_ShouldFindExisting()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await sut.FindFirstOptionalAsync(new ByIdFilterSpecification<UserModel, UserId>(entity.Id), Cancellation.Token);

        actual.Should().HaveSomeValue();
    }

    [Fact]
    public async Task FindFirstOptionalAsync_ShouldNotFindNonExisting()
    {
        var id = Fixture.Create<UserId>();
        var sut = CreateSut();

        var actual = await sut.FindFirstOptionalAsync(new ByIdFilterSpecification<UserModel, UserId>(id), Cancellation.Token);

        actual.Should().HaveNoValue();
    }

    [Fact]
    public async Task HasAsync_ShouldFindExisting()
    {
        var entity = Fixture.Create<User>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await sut.HasAsync(new ByIdFilterSpecification<UserModel, UserId>(entity.Id), Cancellation.Token);

        actual.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnExpected()
    {
        var model = Fixture.Create<UserModel>();
        var sut = CreateSut();
        await Context.Set<UserModel>().AddAsync(model, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await sut.GetAllAsync(Cancellation.Token).ToArrayAsync(Cancellation.Token);

        actual.Should().ContainSingle();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnFalse_WhenNoModels()
    {
        var sut = CreateSut();

        var actual = await sut.AnyAsync(Cancellation.Token);

        actual.Should().BeFalse();
    }

    [Fact]
    public async Task AnyAsync_ShouldReturnTrue_WhenModelsStored()
    {
        var model = Fixture.Create<UserModel>();
        var sut = CreateSut();
        await Context.Set<UserModel>().AddAsync(model, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await sut.AnyAsync(Cancellation.Token);

        actual.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task CountAsync_ShouldReturnCorrectCount(int expectedCount)
    {
        var models = Fixture.CreateMany<UserModel>(expectedCount);
        var sut = CreateSut();
        await Context.Set<UserModel>().AddRangeAsync(models, Cancellation.Token);
        await Context.SaveChangesAsync(Cancellation.Token);

        var actual = await sut.CountAsync(Cancellation.Token);

        actual.Should().Be(expectedCount);
    }

    private FakeRepository CreateSut() => new(Context, Mapper, PersistenceService);

    private class FakeRepository : Repository<UserModel, User, UserId>
    {
        public FakeRepository(IDatabaseContext context, IMapper mapper, IEventsPersistenceService persistenceService)
            : base(context, mapper, SpecificationEvaluator.Default, persistenceService)
        {
        }
    }
}
