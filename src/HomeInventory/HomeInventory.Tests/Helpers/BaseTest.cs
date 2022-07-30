using System.Runtime.Serialization;
using AutoFixture;

namespace HomeInventory.Tests.Helpers;

public abstract class BaseTest : IDisposable
{
    private readonly CancellationTokenSource _source = new();

    protected BaseTest()
    {
    }

    protected IFixture Fixture { get; } = new Fixture();

    protected CancellationToken CancellationToken => _source.Token;

    protected void Cancel() => _source.Cancel();

    protected static object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(type)!;

        // Type without parameterless constructor
        return FormatterServices.GetUninitializedObject(type);
    }

    void IDisposable.Dispose()
    {
        _source.Dispose();
        GC.SuppressFinalize(this);
    }
}
