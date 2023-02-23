using HomeInventory.Domain.Errors;
using MediatR;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, OneOf<TResponse, IError>>
    where TCommand : ICommand<TResponse>
{
}

internal interface ICommandHandler<TCommand> : ICommandHandler<TCommand, Success>
    where TCommand : ICommand
{
}
