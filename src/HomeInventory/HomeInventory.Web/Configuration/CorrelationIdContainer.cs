using HomeInventory.Web.Configuration.Interfaces;

namespace HomeInventory.Web.Configuration;

internal class CorrelationIdContainer : ICorrelationIdContainer
{
    public string CorrelationId { get; private set; } = CreateNewId();

    public void GenerateNew() => SetExisting(CreateNewId());

    public void SetExisting(string id) => CorrelationId = id;

    private static string CreateNewId() => Guid.NewGuid().ToString("N");
}
