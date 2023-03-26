using FluentResults;

namespace HomeInventory.Application.Interfaces.Messaging;

internal abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    protected CommandHandler()
    {
    }

    public async Task<Result<TResponse>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return await InternalHandle(request, cancellationToken);
    }

    protected abstract Task<Result<TResponse>> InternalHandle(TCommand command, CancellationToken cancellationToken);
}

internal abstract class CommandHandler<TCommand> : CommandHandler<TCommand, Success>, ICommandHandler<TCommand>
   where TCommand : ICommand
{
    protected CommandHandler()
    {
    }
}