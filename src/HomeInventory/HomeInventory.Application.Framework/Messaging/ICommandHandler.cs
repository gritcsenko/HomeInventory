namespace HomeInventory.Application.Framework.Messaging;

public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Option<Error>>
    where TCommand : ICommand
{
}
