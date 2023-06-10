using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Persistence;

internal class StorageAreaRepository : Repository<StorageAreaModel, StorageArea>, IStorageAreaRepository
{
    public StorageAreaRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator)
        : base(context, mapper, evaluator)
    {
    }
}
