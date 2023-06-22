using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, OneOf<TResponse, IError>>
    where TQuery : IQuery<TResponse>
{
}
