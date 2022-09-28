using ErrorOr;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>
{
}

