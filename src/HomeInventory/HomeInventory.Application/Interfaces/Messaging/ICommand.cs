using HomeInventory.Domain.Primitives.Errors;
using MediatR;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface ICommand : IRequest<OneOf<Success, IError>>
{
}
