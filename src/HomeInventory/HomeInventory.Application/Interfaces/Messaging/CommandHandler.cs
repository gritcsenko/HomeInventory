using HomeInventory.Domain.Primitives.Errors;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Messaging;

internal abstract class CommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    protected CommandHandler()
    {
    }

    public async Task<OneOf<TResponse, IError>> Handle(TCommand request, CancellationToken cancellationToken)
    {
        return await InternalHandle(request, cancellationToken);
    }

    protected abstract Task<OneOf<TResponse, IError>> InternalHandle(TCommand command, CancellationToken cancellationToken);
}

internal abstract class CommandHandler<TCommand> : CommandHandler<TCommand, Success>, ICommandHandler<TCommand>
   where TCommand : ICommand
{
    protected CommandHandler()
    {
    }
}
