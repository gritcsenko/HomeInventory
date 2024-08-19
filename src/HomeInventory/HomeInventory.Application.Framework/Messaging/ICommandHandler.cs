namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Option<Error>>
    where TCommand : ICommand
{
}
