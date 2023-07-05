using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, OneOf<Success, IError>>
    where TCommand : ICommand
{
}
