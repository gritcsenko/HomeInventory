using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace HomeInventory.Domain.Primitives.Events;

public sealed class EventHub : Disposable, IEventHub
{
    private readonly ConcurrentDictionary<Type, IDisposable> _publishers = new();

    public void Notify<TEvent>(TEvent @event)
        where TEvent : IEvent
    {
        var publisher = GetPublisher<TEvent>();
        publisher.OnNext(@event);
    }

    public IDisposable Subscribe<TEvent>(IObserver<TEvent> observer)
        where TEvent : IEvent =>
        Subscribe<TEvent>(o => o.Subscribe(observer));

    public IDisposable Subscribe<TEvent>(Func<IObservable<TEvent>, IDisposable> subscribeFunc)
        where TEvent : IEvent
    {
        var publisher = GetPublisher<TEvent>();
        return subscribeFunc(publisher);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Dispose(_publishers.Values);
            _publishers.Clear();
        }

        base.Dispose(disposing);
    }

    private Subject<TEvent> GetPublisher<TEvent>() where TEvent : IEvent
    {
        EnsureNotDisposingOrDisposed();
        var key = typeof(TEvent);
        var publisher = _publishers.GetOrAdd(key, CreatePublisher);
        return (Subject<TEvent>)publisher;
    }

    private void EnsureNotDisposingOrDisposed() => ObjectDisposedException.ThrowIf(IsDisposingOrDisposed, nameof(EventHub));

    private IDisposable CreatePublisher(Type type)
    {
        var t = typeof(Subject<>).MakeGenericType(type);
        var instance = Activator.CreateInstance(t);
        return instance as IDisposable ?? throw new InvalidCastException($"Failed to cast {instance?.GetType()} to {typeof(IDisposable)}");
    }
}
