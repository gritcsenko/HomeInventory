using FluentResults;

namespace HomeInventory.Application.Interfaces.Messaging;

internal abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    protected QueryHandler()
    {
    }

    public async Task<Result<TResponse>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        return await InternalHandle(request, cancellationToken);
    }

    protected abstract Task<Result<TResponse>> InternalHandle(TQuery query, CancellationToken cancellationToken);
}
