﻿using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
    protected QueryHandler()
    {
    }

    public async Task<OneOf<TResponse, IError>> Handle(TQuery request, CancellationToken cancellationToken)
    {
        return await InternalHandle(request, cancellationToken);
    }

    protected abstract Task<OneOf<TResponse, IError>> InternalHandle(TQuery query, CancellationToken cancellationToken);
}
