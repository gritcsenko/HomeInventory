namespace HomeInventory.Web.Configuration.Interfaces;

public interface ICorrelationIdContainer
{
    string CorrelationId { get; }

    void GenerateNew();

    void SetExisting(string id);
}
