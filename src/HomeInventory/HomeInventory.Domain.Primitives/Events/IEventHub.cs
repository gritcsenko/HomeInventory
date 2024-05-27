namespace HomeInventory.Domain.Primitives.Events;

public interface IEventHub : IDisposable
{
    void Notify<TEvent>(TEvent @event) where TEvent : IEvent;

    IDisposable Subscribe<TEvent>(IObserver<TEvent> observer) where TEvent : IEvent;

    IDisposable Subscribe<TEvent>(Func<IObservable<TEvent>, IDisposable> subscribeFunc) where TEvent : IEvent;
}
