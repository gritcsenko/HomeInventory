﻿using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface ICommandHandler<TCommand> : IRequestHandler<TCommand, OneOf<Success, IError>>
    where TCommand : ICommand
{
}
