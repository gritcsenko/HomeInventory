namespace HomeInventory.Web.ErrorHandling.Interfaces;

public interface ICorrelationIdContainer
{
    string CorrelationId { get; }

    void GenerateNew();

    void SetExisting(string id);
}
