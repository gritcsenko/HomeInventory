﻿using HomeInventory.Domain.Errors;
using MediatR;
using OneOf;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, OneOf<TResponse, IError>>
    where TQuery : IQuery<TResponse>
{
}
