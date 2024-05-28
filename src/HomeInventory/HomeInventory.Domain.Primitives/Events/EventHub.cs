using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace HomeInventory.Domain.Primitives.Events;

public sealed class EventHub(ISupplier<Cuid> supplier, TimeProvider timeProvider) : Disposable, IEventHub
{
    private readonly ConcurrentDictionary<Type, object> _observables = new();

    public ISupplier<Cuid> EventIdSupplier { get; } = supplier;

    public TimeProvider EventCreatedTimeProvider { get; } = timeProvider;

    public void Inject<TEvent>(IObservable<TEvent> events)
        where TEvent : IEvent
    {
        ObjectDisposedException.ThrowIf(IsDisposingOrDisposed, nameof(EventHub));
        _observables.AddOrUpdate(typeof(TEvent), (_, e) => e.Concat(Observable.Never<TEvent>()), UpdateEvents, events);
    }

    public IObservable<TEvent> GetEvents<TEvent>()
        where TEvent : IEvent
    {
        ObjectDisposedException.ThrowIf(IsDisposingOrDisposed, nameof(EventHub));
        return (ISubject<TEvent>)_observables.GetOrAdd(typeof(TEvent), _ => Observable.Never<TEvent>());
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Dispose(_observables.Values.OfType<IDisposable>());
        }

        base.Dispose(disposing);
    }

    private static IObservable<TEvent> UpdateEvents<TEvent>(Type _, object value, IObservable<TEvent> events)
        where TEvent : IEvent
    {
        var existing = (IObservable<TEvent>)value;
        return existing.Merge(events);
    }
}
