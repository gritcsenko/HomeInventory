using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class RepositoryTests : BaseRepositoryTest
{
    public RepositoryTests()
    {
    }

    [Fact]
    public async ValueTask WithUnitOfWorkAsync_ShouldReturnBeSame_WhenCalledSecondTime()
    {
        var sut = CreateSut();
        var expected = await sut.WithUnitOfWorkAsync(Cancellation.Token);

        var actual = await sut.WithUnitOfWorkAsync(Cancellation.Token);

        actual.Should().BeSameAs(expected);
    }

    [Fact]
    public async ValueTask WithUnitOfWorkAsync_ShouldReturnBeDifferent_WhenCalledSecondTimeAndDisposedFirst()
    {
        var sut = CreateSut();
        var first = await sut.WithUnitOfWorkAsync(Cancellation.Token);
        await first.DisposeAsync();

        var actual = await sut.WithUnitOfWorkAsync(Cancellation.Token);

        actual.Should().NotBeSameAs(first);
    }

    [Fact]
    public async ValueTask AddAsync_ShouldReturnSameEntityAsProvided()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();

        var actual = await sut.AddAsync(entity, Cancellation.Token);

        actual.Should().BeSameAs(entity);
    }

    [Fact]
    public async ValueTask AddAsync_ShouldAdd_WhenUsingUnitOfWork()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();
        await using var unit = await sut.WithUnitOfWorkAsync(Cancellation.Token);

        var _ = await sut.AddAsync(entity, Cancellation.Token);

        await unit.SaveChangesAsync(Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().ContainSingle();
    }

    [Fact]
    public async ValueTask AddAsync_ShouldAdd_WhenNotUsingUnitOfWork()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();

        var _ = await sut.AddAsync(entity, Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().ContainSingle();
    }

    [Fact]
    public async ValueTask AddRangeAsync_ShouldAdd_WhenUsingUnitOfWork()
    {
        var entities = Fixture.CreateMany<FakeEntity>();
        var sut = CreateSut();
        await using var unit = await sut.WithUnitOfWorkAsync(Cancellation.Token);

        await sut.AddRangeAsync(entities, Cancellation.Token);

        await unit.SaveChangesAsync(Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async ValueTask AddRangeAsync_ShouldAdd_WhenNotUsingUnitOfWork()
    {
        var entities = Fixture.CreateMany<FakeEntity>();
        var sut = CreateSut();

        await sut.AddRangeAsync(entities, Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async ValueTask GetAllAsync_ShouldReturnExpected()
    {
        var model = Fixture.Create<FakeModel>();
        var sut = CreateSut();
        await Context.Set<FakeModel>().AddAsync(model, Cancellation.Token);

        var actual = await sut.GetAllAsync(Cancellation.Token).ToArrayAsync(Cancellation.Token);

        actual.Should().ContainSingle();
    }

    [Fact]
    public async ValueTask AnyAsync_ShouldReturnFalse_WhenNoModels()
    {
        var sut = CreateSut();

        var actual = await sut.AnyAsync(Cancellation.Token);

        actual.Should().BeFalse();
    }

    [Fact]
    public async ValueTask AnyAsync_ShouldReturnTrue_WhenModelsStored()
    {
        var model = Fixture.Create<FakeModel>();
        var sut = CreateSut();
        await Context.Set<FakeModel>().AddAsync(model, Cancellation.Token);

        var actual = await sut.AnyAsync(Cancellation.Token);

        actual.Should().BeTrue();
    }

    private FakeRepository CreateSut() => new(Factory, Mapper, SpecificationEvaluator.Default);

    private class FakeRepository : Repository<FakeModel, FakeEntity>
    {
        public FakeRepository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator)
            : base(contextFactory, mapper, evaluator)
        {
        }
    }

    private class FakeModel : IPersistentModel
    {
        public Guid Id { get; init; }
    }

    private class FakeEntity : HomeInventory.Domain.Primitives.IEntity<FakeEntity>
    {
    }
}
