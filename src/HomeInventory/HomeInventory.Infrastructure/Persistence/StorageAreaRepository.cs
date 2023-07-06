using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence;

internal class StorageAreaRepository : Repository<StorageAreaModel, StorageArea, StorageAreaId>, IStorageAreaRepository
{
    public StorageAreaRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator, IEventsPersistenceService eventsPersistenceService)
        : base(context, mapper, evaluator, eventsPersistenceService)
    {
    }
}
