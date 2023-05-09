﻿using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, OneOf<TResponse, IError>>
    where TCommand : ICommand<TResponse>
{
}

public interface ICommandHandler<TCommand> : ICommandHandler<TCommand, Success>
    where TCommand : ICommand
{
}
