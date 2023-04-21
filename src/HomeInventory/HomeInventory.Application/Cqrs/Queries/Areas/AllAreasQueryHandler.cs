using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Cqrs.Queries.Areas;

internal class AllAreasQueryHandler : QueryHandler<AllAreasQuery, AreasResult>
{
    private readonly IStorageAreaRepository _repository;

    public AllAreasQueryHandler(IStorageAreaRepository repository)
    {
        _repository = repository;
    }

    protected override async Task<OneOf<AreasResult, IError>> InternalHandle(AllAreasQuery query, CancellationToken cancellationToken)
    {
        var areas = await _repository.GetAllAsync(cancellationToken).ToArrayAsync(cancellationToken);
        return new AreasResult(areas);
    }
}
