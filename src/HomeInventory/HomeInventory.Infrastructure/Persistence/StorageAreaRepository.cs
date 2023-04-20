using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class StorageAreaRepository : Repository<StorageAreaModel, StorageArea>, IStorageAreaRepository
{
    public StorageAreaRepository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator, IDateTimeService dateTimeService)
        : base(contextFactory, mapper, evaluator, dateTimeService)
    {
    }
}
