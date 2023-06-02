using MediatR;

namespace HomeInventory.Domain.Primitives;

public interface IEvent : INotification
{
    Guid Id { get; }
}
