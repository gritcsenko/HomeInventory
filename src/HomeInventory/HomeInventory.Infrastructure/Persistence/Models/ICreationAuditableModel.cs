namespace HomeInventory.Infrastructure.Persistence.Models;

public interface ICreationAuditableModel
{
    DateTimeOffset CreatedOn { get; set; }
}
