namespace HomeInventory.Web.Configuration.Interfaces;

public interface ICorrelationIdContainer
{
    string CorrelationId { get; set; }

    void GenerateNew();
}
