namespace HomeInventory.Domain.Primitives.Events;

public interface IEventHub : IDisposable
{
    ISupplier<Cuid> EventIdSupplier { get; }

    TimeProvider EventCreatedTimeProvider { get; }

    void Inject<TEvent>(IObservable<TEvent> events) where TEvent : IEvent;

    IObservable<TEvent> GetEvents<TEvent>() where TEvent : IEvent;
}
