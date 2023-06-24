using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Tests.Systems.Persistence;

[UnitTest]
public class RepositoryTests : BaseRepositoryTest
{
    [Fact]
    public async ValueTask AddAsync_ShouldAdd()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();

        await sut.AddAsync(entity, Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().ContainSingle();
    }

    [Fact]
    public async ValueTask AddRangeAsync_ShouldAdd()
    {
        var entities = Fixture.CreateMany<FakeEntity>();
        var sut = CreateSut();

        await sut.AddRangeAsync(entities, Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async ValueTask DeleteAsync_ShouldRemoveExisting()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);

        await sut.DeleteAsync(entity, Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().BeEmpty();
    }

    [Fact]
    public async ValueTask DeleteRangeAsync_ShouldRemoveExisting()
    {
        var entities = Fixture.CreateMany<FakeEntity>();
        var sut = CreateSut();
        await sut.AddRangeAsync(entities, Cancellation.Token);

        await sut.DeleteRangeAsync(entities, Cancellation.Token);

        var actual = await Context.Set<FakeModel>().ToArrayAsync(Cancellation.Token);
        actual.Should().HaveSameCount(entities);
    }

    [Fact]
    public async ValueTask FindFirstOptionalAsync_ShouldFindExisting()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);

        var actual = await sut.FindFirstOptionalAsync(new ByIdFilterSpecification<FakeModel, Ulid>(entity.Id.Value), Cancellation.Token);

        actual.Should().HaveSomeValue();
    }

    [Fact]
    public async ValueTask FindFirstOptionalAsync_ShouldNotFindNonExisting()
    {
        var entityId = Fixture.Create<Ulid>();
        var sut = CreateSut();

        var actual = await sut.FindFirstOptionalAsync(new ByIdFilterSpecification<FakeModel, Ulid>(entityId), Cancellation.Token);

        actual.Should().HaveNoValue();
    }

    [Fact]
    public async ValueTask HasAsync_ShouldFindExisting()
    {
        var entity = Fixture.Create<FakeEntity>();
        var sut = CreateSut();
        await sut.AddAsync(entity, Cancellation.Token);

        var actual = await sut.HasAsync(new ByIdFilterSpecification<FakeModel, Ulid>(entity.Id.Value), Cancellation.Token);

        actual.Should().BeTrue();
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

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async ValueTask CountAsync_ShouldReturnCorrectCount(int expectedCount)
    {
        var models = Fixture.CreateMany<FakeModel>(expectedCount);
        var sut = CreateSut();
        await Context.Set<FakeModel>().AddRangeAsync(models, Cancellation.Token);

        var actual = await sut.CountAsync(Cancellation.Token);

        actual.Should().Be(expectedCount);
    }

    private FakeRepository CreateSut() => new(Context, Mapper);

    private class FakeRepository : Repository<FakeModel, FakeEntity>
    {
        public FakeRepository(IDatabaseContext context, IMapper mapper)
            : base(context, mapper, SpecificationEvaluator.Default)
        {
        }
    }

    private class FakeModel : IPersistentModel
    {
        public required Ulid Id { get; init; }
    }

#pragma warning disable CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
    private class FakeEntity : IEntity<FakeEntity, FakeId>, IHasDomainEvents
#pragma warning restore CA1067 // Override Object.Equals(object) when implementing IEquatable<T>
    {
        public required FakeId Id { get; init; }

        private readonly IReadOnlyCollection<IDomainEvent> _domainEvents = Array.Empty<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents;

        public bool Equals(FakeEntity? other) => throw new NotImplementedException();

        public void ClearDomainEvents()
        {
            // Nothing to do here
        }
    }

    private class FakeId : UlidIdentifierObject<FakeId>
    {
        public FakeId(Ulid value)
            : base(value)
        {
        }
    }
}
