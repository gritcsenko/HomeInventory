using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace HomeInventory.Core;

/// <summary>
/// Provides implementation of dispose pattern.
/// </summary>
/// <seealso cref="IDisposable"/>
/// <seealso cref="IAsyncDisposable"/>
/// <seealso href="https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose">Implementing Dispose method</seealso>
[SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "False positive")]
public abstract class DisposableBase : IDisposable
{
    private const int _notDisposedState = 0;
    private const int _disposingState = 1;
    private const int _disposedState = 2;

    private volatile int _state;

    /// <summary>
    /// Indicates that this object is disposed.
    /// </summary>
    protected bool IsDisposed => _state is _disposedState;

    /// <summary>
    /// Indicates that <see cref="DisposeAsync()"/> is called but not yet completed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool IsDisposing => _state is _disposingState;

    /// <summary>
    /// Indicates that <see cref="DisposeAsync()"/> is called.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool IsDisposingOrDisposed => _state is not _notDisposedState;

    private string ObjectName => GetType().Name;

    /// <summary>
    /// Gets a task representing <see cref="ObjectDisposedException"/> exception.
    /// </summary>
    protected Task GetDisposedTask() =>
        Task.FromException(new ObjectDisposedException(ObjectName));

    /// <summary>
    /// Returns a task representing <see cref="ObjectDisposedException"/> exception.
    /// </summary>
    /// <typeparam name="T">The type of the task.</typeparam>
    /// <returns>The task representing <see cref="ObjectDisposedException"/> exception.</returns>
    protected Task<T> GetDisposedTask<T>()
        => Task.FromException<T>(new ObjectDisposedException(ObjectName));

    /// <summary>
    /// Attempts to complete the task with <see cref="ObjectDisposedException"/> exception.
    /// </summary>
    /// <param name="source">The task completion source.</param>
    /// <typeparam name="T">The type of the task.</typeparam>
    /// <returns><see langword="true"/> if operation was successful; otherwise, <see langword="false"/>.</returns>
    protected bool TrySetDisposedException<T>(TaskCompletionSource<T> source)
        => source.TrySetException(new ObjectDisposedException(ObjectName));

    /// <summary>
    /// Attempts to complete the task with <see cref="ObjectDisposedException"/> exception.
    /// </summary>
    /// <param name="source">The task completion source.</param>
    /// <returns><see langword="true"/> if operation was successful; otherwise, <see langword="false"/>.</returns>
    protected bool TrySetDisposedException(TaskCompletionSource source)
        => source.TrySetException(new ObjectDisposedException(ObjectName));

    /// <summary>
    /// Releases managed and unmanaged resources associated with this object.
    /// </summary>
    /// <param name="disposing"><see langword="true"/> if called from <see cref="Dispose()"/>; <see langword="false"/> if called from finalizer <see cref="Finalize()"/>.</param>
    protected virtual void Dispose(bool disposing)
        => _state = _disposedState;

    /// <summary>
    /// Releases managed resources associated with this object asynchronously.
    /// </summary>
    /// <remarks>
    /// This method makes sense only if derived class implements <see cref="IAsyncDisposable"/> interface.
    /// </remarks>
    /// <returns>The task representing asynchronous execution of this method.</returns>
    protected virtual ValueTask DisposeAsyncCore()
    {
#pragma warning disable CA1849 // Call async methods when in an async method
        Dispose(true);
#pragma warning restore CA1849 // Call async methods when in an async method
        return ValueTask.CompletedTask;
    }

    private async ValueTask DisposeAsyncImpl()
    {
        await DisposeAsyncCore().ConfigureAwait(false);

#pragma warning disable CA1849 // Call async methods when in an async method
        Dispose(false);
#pragma warning restore CA1849 // Call async methods when in an async method

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
#pragma warning disable S3971 // "GC.SuppressFinalize" should not be called
        GC.SuppressFinalize(this);
#pragma warning restore S3971 // "GC.SuppressFinalize" should not be called
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
    }

    /// <summary>
    /// Releases managed resources associated with this object asynchronously.
    /// </summary>
    /// <remarks>
    /// If derived class implements <see cref="IAsyncDisposable"/> then <see cref="IAsyncDisposable.DisposeAsync"/>
    /// can be trivially implemented through delegation of the call to this method.
    /// </remarks>
    /// <returns>The task representing asynchronous execution of this method.</returns>
    protected ValueTask DisposeAsync() => Interlocked.CompareExchange(ref _state, _disposingState, _notDisposedState) switch
    {
        _notDisposedState => DisposeAsyncImpl(),
        _disposingState => DisposeAsyncCore(),
        _ => ValueTask.CompletedTask,
    };

    /// <summary>
    /// Starts disposing this object.
    /// </summary>
    /// <returns><see langword="true"/> if cleanup operations can be performed; <see langword="false"/> if the object is already disposing.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected bool TryBeginDispose()
        => Interlocked.CompareExchange(ref _state, _disposingState, _notDisposedState) is _notDisposedState;

    /// <summary>
    /// Disposes many objects.
    /// </summary>
    /// <param name="objects">An array of objects to dispose.</param>
    [SuppressMessage("Blocker Code Smell", "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"", Justification = "By Design")]
    public static void Dispose(IEnumerable<IDisposable?> objects)
    {
        foreach (var obj in objects)
            obj?.Dispose();
    }

    /// <summary>
    /// Disposes many objects in safe manner.
    /// </summary>
    /// <param name="objects">An array of objects to dispose.</param>
    [SuppressMessage("Blocker Code Smell", "S2953:Methods named \"Dispose\" should implement \"IDisposable.Dispose\"", Justification = "By Design")]
    public static void Dispose(ReadOnlySpan<IDisposable?> objects)
    {
        foreach (var obj in objects)
            obj?.Dispose();
    }

    /// <summary>
    /// Releases all resources associated with this object.
    /// </summary>
    [SuppressMessage("Design", "CA1063", Justification = "No need to call Dispose(true) multiple times")]
    public void Dispose()
    {
        Dispose(TryBeginDispose());
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes many objects.
    /// </summary>
    /// <param name="objects">An array of objects to dispose.</param>
    /// <returns>The task representing asynchronous execution of this method.</returns>
    public static async ValueTask DisposeAsync(IEnumerable<IAsyncDisposable?> objects)
    {
#pragma warning disable S3267 // Loops should be simplified with "LINQ" expressions
        foreach (var obj in objects)
        {
            if (obj is not null)
                await obj.DisposeAsync().ConfigureAwait(false);
        }
#pragma warning restore S3267 // Loops should be simplified with "LINQ" expressions
    }

    /// <summary>
    /// Disposes many objects in safe manner.
    /// </summary>
    /// <param name="objects">An array of objects to dispose.</param>
    /// <returns>The task representing asynchronous execution of this method.</returns>
    public static ValueTask DisposeAsync(params IAsyncDisposable?[] objects)
        => DisposeAsync(objects.AsEnumerable());

    /// <summary>
    /// Finalizes this object.
    /// </summary>
    ~DisposableBase() => Dispose(false);
}