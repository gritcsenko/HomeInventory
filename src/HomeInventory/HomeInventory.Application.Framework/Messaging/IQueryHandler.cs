namespace HomeInventory.Application.Framework.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IQueryResult<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
}
