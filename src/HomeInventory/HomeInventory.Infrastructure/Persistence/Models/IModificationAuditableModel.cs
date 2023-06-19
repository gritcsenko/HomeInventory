namespace HomeInventory.Infrastructure.Persistence.Models;

public interface IModificationAuditableModel
{
    DateTimeOffset ModifiedOn { get; set; }
}
