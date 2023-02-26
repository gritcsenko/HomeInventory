using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;

namespace HomeInventory.Tests.Systems.Persistence;

[Trait("Category", "Unit")]
public class RepositoryTests : BaseRepositoryTest
{
    public RepositoryTests()
    {
    }

    [Fact]
    public void UnitOfWork_ShouldBeNone_WhenCreated()
    {
        var sut = CreateSut();

        var actual = sut.UnitOfWork;

        actual.Value.Should().BeOfType<None>();
    }

    [Fact]
    public async ValueTask UnitOfWork_ShouldBeSame_WhenWithUnitOfWorkAsyncCalled()
    {
        var sut = CreateSut();
        var expected = await sut.WithUnitOfWorkAsync(CancellationToken);

        var actual = sut.UnitOfWork;

        actual.Value.Should().Be(expected);
    }

    [Fact]
    public async ValueTask WithUnitOfWorkAsync_ShouldReturnBeSame_WhenCalledSecondTime()
    {
        var sut = CreateSut();
        var expected = await sut.WithUnitOfWorkAsync(CancellationToken);

        var actual = await sut.WithUnitOfWorkAsync(CancellationToken);

        actual.Should().BeSameAs(expected);
    }

    private FakeRepository CreateSut() => new(Factory, Mapper, SpecificationEvaluator.Default, DateTimeService);

    private class FakeRepository : Repository<FakeModel, FakeEntity>
    {
        public FakeRepository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator, IDateTimeService dateTimeService)
            : base(contextFactory, mapper, evaluator, dateTimeService)
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
