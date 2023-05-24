using DotNext;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class UnitOfWorkContainer : IResettable
{
    private readonly IDbContextFactory<DatabaseContext> _factory;
    private readonly IDateTimeService _dateTimeService;
    private Optional<UnitOfWork> _unitOfWork = Optional.None<UnitOfWork>();

    public UnitOfWorkContainer(IDbContextFactory<DatabaseContext> factory, IDateTimeService dateTimeService)
    {
        _factory = factory;
        _dateTimeService = dateTimeService;
    }

    public IAsyncDisposable Resource { get; private set; } = EmptyAsyncDisposable.Instance;

    public async Task<DbContext> EnsureAsync(CancellationToken cancellationToken = default)
    {
        Resource = EmptyAsyncDisposable.Instance;
        if (_unitOfWork.HasValue)
        {
            return _unitOfWork.Value.DbContext;
        }

        var unit = await CreateNewAsync(cancellationToken);
        return unit.DbContext;
    }

    public async Task<UnitOfWork> CreateNewAsync(CancellationToken cancellationToken = default)
    {
        var context = await _factory.CreateDbContextAsync(cancellationToken);
        var unit = new UnitOfWork(context, _dateTimeService, new ReleaseUnitOfWork(this));
        _unitOfWork = unit;
        Resource = unit;
        return unit;
    }

    void IResettable.Reset()
    {
        _unitOfWork = Optional.None<UnitOfWork>();
        Resource = EmptyAsyncDisposable.Instance;
    }

    private sealed class ReleaseUnitOfWork : IDisposable
    {
        private readonly IResettable _resettable;

        public ReleaseUnitOfWork(IResettable resettable) => _resettable = resettable;

        void IDisposable.Dispose() => _resettable.Reset();
    }
}
