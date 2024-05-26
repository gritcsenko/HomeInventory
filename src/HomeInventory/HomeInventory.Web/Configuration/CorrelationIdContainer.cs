using DotNext;
using HomeInventory.Web.Configuration.Interfaces;
using Visus.Cuid;

namespace HomeInventory.Web.Configuration;

internal class CorrelationIdContainerr : ICorrelationIdContainer
{
    private readonly ISupplier<Cuid> _supplier;

    public CorrelationIdContainerr(ISupplier<Cuid> supplier)
    {
        _supplier = supplier;
        CorrelationId = CreateNewId();
    }

    public string CorrelationId { get; private set; }

    public void GenerateNew() => SetExisting(CreateNewId());

    public void SetExisting(string id) => CorrelationId = id;

    private string CreateNewId() => _supplier.Invoke().ToString();
}
