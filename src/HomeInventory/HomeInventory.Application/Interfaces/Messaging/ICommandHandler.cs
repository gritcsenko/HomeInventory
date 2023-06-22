using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface ICommandHandler<TCommand> : IRequestHandler<TCommand, OneOf<Success, IError>>
    where TCommand : ICommand
{
}
