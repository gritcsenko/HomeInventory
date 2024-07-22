using DotNext;
using HomeInventory.Web.Configuration.Interfaces;

namespace HomeInventory.Web.Configuration;

internal class CorrelationIdContainer : ICorrelationIdContainer
{
    private readonly ISupplier<Ulid> _supplier;

    public CorrelationIdContainer(ISupplier<Ulid> supplier)
    {
        _supplier = supplier;
        CorrelationId = CreateNewId();
    }

    public string CorrelationId { get; private set; }

    public void GenerateNew() => SetExisting(CreateNewId());

    public void SetExisting(string id) => CorrelationId = id;

    private string CreateNewId() => _supplier.Invoke().ToString();
}
