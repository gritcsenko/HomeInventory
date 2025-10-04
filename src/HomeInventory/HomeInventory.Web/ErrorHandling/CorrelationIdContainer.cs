using HomeInventory.Web.ErrorHandling.Interfaces;

namespace HomeInventory.Web.ErrorHandling;

internal class CorrelationIdContainer : ICorrelationIdContainer
{
    public string CorrelationId { get; private set; } = CreateNewId();

    public void GenerateNew() => SetExisting(CreateNewId());

    public void SetExisting(string id) => CorrelationId = id;

    private static string CreateNewId() => Ulid.NewUlid().ToString();
}
