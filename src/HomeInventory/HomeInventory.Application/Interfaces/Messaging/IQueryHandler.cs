using FluentResults;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, IResult<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
