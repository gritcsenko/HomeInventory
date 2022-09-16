namespace HomeInventory.Web.Configuration.Interfaces;
public interface ICorrelationIdGenerator
{
    string GetCorrelationId();

    void SetCorrelationId(string correlationId);
}
