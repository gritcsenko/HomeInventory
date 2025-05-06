namespace HomeInventory.Application.Framework.Messaging;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    public async Task<Option<Error>> Handle(TCommand request, CancellationToken cancellationToken) =>
        await InternalHandle(request, cancellationToken);

    protected abstract Task<Option<Error>> InternalHandle(TCommand command, CancellationToken cancellationToken);
}
