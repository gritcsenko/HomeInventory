using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace HomeInventory.Domain.Primitives.Events;

public abstract class EventHandlerBase<TEvent> : Disposable
    where TEvent : IEvent
{
    private readonly CancellationTokenSource _source = new();
    private readonly Subject<(IEventHub Hub, TEvent Event)> _subject = new();
    private readonly IDisposable _subscription;

    protected EventHandlerBase()
    {
        _subscription = _subject
            .Select(e => Observable.Defer(() => HandleEventAsync(e.Hub, e.Event, _source.Token).ToObservable()))
            .Concat()
            .Subscribe();
    }

    public IDisposable Subscribe(IEventHub hub) => hub.Subscribe<TEvent>(o => o.Select(e => (Hub: hub, Evente: e)).Subscribe(_subject));

    protected abstract Task HandleEventAsync(IEventHub hub, TEvent @event, CancellationToken cancellationToken);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _subscription.Dispose();
            _source.Dispose();
            _subject.Dispose();
        }

        base.Dispose(disposing);
    }
}
