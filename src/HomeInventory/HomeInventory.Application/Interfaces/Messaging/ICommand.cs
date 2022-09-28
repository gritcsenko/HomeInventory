using ErrorOr;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>
{
}

public interface ICommand : ICommand<Success>
{
}
