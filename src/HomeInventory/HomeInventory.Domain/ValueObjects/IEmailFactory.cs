using FluentResults;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

public interface IEmailFactory
{
    OneOf<Email, IError> CreateFrom(string value);
}
