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
