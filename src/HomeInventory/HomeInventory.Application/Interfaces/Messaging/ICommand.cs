using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Interfaces.Messaging;

internal interface ICommand : IRequest<OneOf<Success, IError>>
{
}
