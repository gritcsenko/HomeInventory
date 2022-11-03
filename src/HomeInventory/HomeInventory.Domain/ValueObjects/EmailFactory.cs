using FluentResults;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Domain.ValueObjects;

internal class EmailFactory : ValueObjectFactory<Email>, IEmailFactory
{
    public IResult<Email> CreateFrom(string value) => TryCreate(value, IsEmailValid, x => new Email(x));

    private bool IsEmailValid(string value)
    {
        return !string.IsNullOrEmpty(value);
    }
}
