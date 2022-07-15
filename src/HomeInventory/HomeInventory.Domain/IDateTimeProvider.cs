using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("HomeInventory.Tests")]

namespace HomeInventory.Domain;

public interface IDateTimeService
{
    DateTimeOffset Now { get; }
    DateOnly Today { get; }
}
