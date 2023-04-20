using FluentResults;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result<Success>>
    where TCommand : ICommand
{
}
