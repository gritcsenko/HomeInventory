using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface IQuery<TResponse> : IRequest<OneOf<TResponse, IError>>
{
}
