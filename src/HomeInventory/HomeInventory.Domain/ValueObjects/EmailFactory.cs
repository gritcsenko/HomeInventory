using FluentResults;
using HomeInventory.Domain.Primitives;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal class EmailFactory : ValueObjectFactory<Email>, IValueObjectFactory<Email, string>
{
    public OneOf<Email, IError> CreateFrom(string value) => TryCreate(value, IsEmailValid, x => new Email(x));

    private bool IsEmailValid(string value) => !string.IsNullOrEmpty(value);
}