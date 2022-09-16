using HomeInventory.Web.Configuration.Interfaces;

namespace HomeInventory.Web.Configuration;
internal class CorrelationIdGenerator : ICorrelationIdGenerator
{
    private string _correlationId = Guid.NewGuid().ToString("N");


    public string GetCorrelationId() => _correlationId;

    public void SetCorrelationId(string correlationId) => _correlationId = correlationId;
}
