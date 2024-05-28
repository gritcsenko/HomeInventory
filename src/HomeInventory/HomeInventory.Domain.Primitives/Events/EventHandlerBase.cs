using System.Reactive;
using System.Reactive.Linq;

namespace HomeInventory.Domain.Primitives.Events;

public abstract class EventHandlerBase<TEvent, TResult>()
    where TEvent : IEvent
{
    public IDisposable Subscribe(IEventHub hub) =>
        InternalModify(hub, hub.GetEvents<TEvent>()).Subscribe();

    protected virtual IObservable<TResult> InternalModify(IEventHub hub, IObservable<TEvent> events) => events.SelectMany(e => HandleEvent(hub, e));

    protected abstract IObservable<TResult> HandleEvent(IEventHub hub, TEvent @event);
}

public abstract class EventHandlerBase<TEvent>() : EventHandlerBase<TEvent, Unit>
    where TEvent : IEvent
{
    protected override IObservable<Unit> HandleEvent(IEventHub hub, TEvent @event) =>
        Observable.FromAsync(ct => HandleEventAsync(hub, @event, ct));

    protected abstract Task HandleEventAsync(IEventHub hub, TEvent @event, CancellationToken cancellationToken);
}
