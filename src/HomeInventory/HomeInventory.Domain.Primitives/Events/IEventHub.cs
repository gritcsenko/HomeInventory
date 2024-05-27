namespace HomeInventory.Domain.Primitives.Events;

public interface IEventHub : IDisposable
{
    void Notify<TEvent>(TEvent @event) where TEvent : IEvent;

    IObservable<TEvent> GetEvents<TEvent>() where TEvent : IEvent;
}
