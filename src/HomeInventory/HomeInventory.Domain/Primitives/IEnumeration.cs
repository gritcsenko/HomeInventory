namespace HomeInventory.Domain.Primitives;

public interface IEnumeration : IValueObject
{
}

public interface IEnumeration<TEnum> : IEnumeration, IValueObject<TEnum>
    where TEnum : notnull, IEnumeration<TEnum>
{
}
