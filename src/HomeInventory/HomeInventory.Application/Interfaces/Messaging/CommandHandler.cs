using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

internal abstract class CommandHandler<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    protected CommandHandler()
    {
    }

    public async Task<OneOf<Success, IError>> Handle(TCommand request, CancellationToken cancellationToken) =>
        await InternalHandle(request, cancellationToken);

    protected abstract Task<OneOf<Success, IError>> InternalHandle(TCommand command, CancellationToken cancellationToken);
}
