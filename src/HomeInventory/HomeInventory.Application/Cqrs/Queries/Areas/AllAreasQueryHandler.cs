using FluentResults;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Persistence;

namespace HomeInventory.Application.Cqrs.Queries.Areas;

internal class AllAreasQueryHandler : QueryHandler<AllAreasQuery, AreasResult>
{
    private readonly IStorageAreaRepository _repository;

    public AllAreasQueryHandler(IStorageAreaRepository repository)
    {
        _repository = repository;
    }

    protected override async Task<Result<AreasResult>> InternalHandle(AllAreasQuery query, CancellationToken cancellationToken)
    {
        var areas = await _repository.GetAllAsync().ToArrayAsync(cancellationToken);
        return new AreasResult(areas);
    }
}
