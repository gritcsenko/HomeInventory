using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using OneOf;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface IQuery<TResponse> : IRequest<OneOf<TResponse, IError>>
{
}
