namespace HomeInventory.Domain.Primitives.Ids;

public interface IIdSupplier<out TId>
{
    TId Supply();
}
