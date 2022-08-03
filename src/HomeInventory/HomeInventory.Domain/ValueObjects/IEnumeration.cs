namespace HomeInventory.Domain.ValueObjects;

public interface IEnumeration : IValueObject
{
}

public interface IEnumeration<TEnum> : IEnumeration, IValueObject<TEnum>
    where TEnum : notnull, IEnumeration<TEnum>
{
}
