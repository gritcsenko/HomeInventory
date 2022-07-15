namespace HomeInventory.Domain.ValueObjects;

public interface IValueValidator<TValue>
{
    bool IsValid(TValue value);
}
