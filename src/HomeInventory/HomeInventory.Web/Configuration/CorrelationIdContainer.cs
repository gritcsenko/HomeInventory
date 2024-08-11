using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Web.Configuration.Interfaces;

namespace HomeInventory.Web.Configuration;

internal class CorrelationIdContainer : ICorrelationIdContainer
{
    private readonly IIdSupplier<Ulid> _supplier;

    public CorrelationIdContainer(IIdSupplier<Ulid> supplier)
    {
        _supplier = supplier;
        CorrelationId = CreateNewId();
    }

    public string CorrelationId { get; private set; }

    public void GenerateNew() => SetExisting(CreateNewId());

    public void SetExisting(string id) => CorrelationId = id;

    private string CreateNewId() => _supplier.Supply().ToString();
}
