using DotNext;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class UnitOfWorkContainer : IResettable
{
    private readonly IDbContextFactory<DatabaseContext> _factory;
    private Optional<UnitOfWork> _unitOfWork = Optional.None<UnitOfWork>();

    public UnitOfWorkContainer(IDbContextFactory<DatabaseContext> factory)
    {
        _factory = factory;
    }

    public IAsyncDisposable Resource { get; private set; } = EmptyAsyncDisposable.Instance;

    public async Task<UnitOfWork> EnsureAsync(CancellationToken cancellationToken = default)
    {
        if (_unitOfWork.HasValue)
        {
            Resource = EmptyAsyncDisposable.Instance;
            return _unitOfWork.Value;
        }

        return await CreateNewAsync(cancellationToken);
    }

    public async Task<UnitOfWork> CreateNewAsync(CancellationToken cancellationToken = default)
    {
        var context = await _factory.CreateDbContextAsync(cancellationToken);
        var unit = new UnitOfWork(context, new ReleaseUnitOfWork(this));
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
