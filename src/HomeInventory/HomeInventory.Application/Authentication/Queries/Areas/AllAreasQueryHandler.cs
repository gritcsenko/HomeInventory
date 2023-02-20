using FluentResults;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Persistence;

namespace HomeInventory.Application.Authentication.Queries.Areas;

internal class AllAreasQueryHandler : IQueryHandler<AllAreasQuery, AreasResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IStorageAreaRepository _repository;

    public AllAreasQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IStorageAreaRepository repository)
    {
        _tokenGenerator = tokenGenerator;
        _repository = repository;
    }

    public async Task<Result<AreasResult>> Handle(AllAreasQuery request, CancellationToken cancellationToken)
    {
        var areas = await _repository.GetAllAsync().ToArrayAsync(cancellationToken);
        return new AreasResult(areas);
    }
}
