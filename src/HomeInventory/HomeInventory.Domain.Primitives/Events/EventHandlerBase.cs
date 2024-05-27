using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace HomeInventory.Domain.Primitives.Events;

public abstract class EventHandlerBase<TEvent>() : Disposable
    where TEvent : IEvent
{
    private readonly CancellationTokenSource _source = new();

    public IDisposable Subscribe(IEventHub hub) =>
        hub.GetEvents<TEvent>().Select(e => HandleEvent(hub, e)).Concat().Subscribe();

    protected abstract Task HandleEventAsync(IEventHub hub, TEvent @event, CancellationToken cancellationToken);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _source.Cancel();
            _source.Dispose();
        }

        base.Dispose(disposing);
    }

    private IObservable<Unit> HandleEvent(IEventHub hub, TEvent @event) =>
        Observable.Defer(() => HandleEventAsync(hub, @event, _source.Token).ToObservable());
}
