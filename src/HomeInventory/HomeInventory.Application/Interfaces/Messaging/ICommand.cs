using FluentResults;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

public interface ICommand : ICommand<Success>
{
}
