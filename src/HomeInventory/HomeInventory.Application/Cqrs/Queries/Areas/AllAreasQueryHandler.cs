using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Persistence;

namespace HomeInventory.Application.Cqrs.Queries.Areas;

internal sealed class AllAreasQueryHandler(IStorageAreaRepository repository) : QueryHandler<AllAreasQuery, AreasResult>
{
    private readonly IStorageAreaRepository _repository = repository;

    protected override async Task<Validation<Error, AreasResult>> InternalHandle(AllAreasQuery query, CancellationToken cancellationToken)
    {
        var areas = await _repository.GetAllAsync(cancellationToken).ToArrayAsync(cancellationToken);
        return new AreasResult(areas);
    }
}
