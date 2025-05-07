namespace HomeInventory.Application.Framework.Messaging;

public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    public async Task<IQueryResult<TResponse>> Handle(TQuery request, CancellationToken cancellationToken = default)
    {
        var validation = await InternalHandle(request, cancellationToken);
        return new QueryResult<TResponse>(validation);
    }

    protected abstract Task<Validation<Error, TResponse>> InternalHandle(TQuery query, CancellationToken cancellationToken);
}