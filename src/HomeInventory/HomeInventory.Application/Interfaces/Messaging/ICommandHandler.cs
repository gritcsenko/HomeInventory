using FluentResults;
using MediatR;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, IResult<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

internal interface ICommandHandler<TCommand> : ICommandHandler<TCommand, Success>
    where TCommand : ICommand
{
}
