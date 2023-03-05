using HomeInventory.Web.Configuration.Interfaces;

namespace HomeInventory.Web.Configuration;

internal class CorrelationIdContainer : ICorrelationIdContainer
{
    public string CorrelationId { get; set; } = CreateNewId();

    public void GenerateNew() => CorrelationId = CreateNewId();

    private static string CreateNewId() => Guid.NewGuid().ToString("N");
}
