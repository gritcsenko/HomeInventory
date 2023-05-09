﻿using FluentResults;
using MediatR;
using OneOf;

namespace HomeInventory.Application.Interfaces.Messaging;

public interface IQuery<TResponse> : IRequest<OneOf<TResponse, IError>>
{
}
